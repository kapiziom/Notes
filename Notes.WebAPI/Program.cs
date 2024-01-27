using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notes.Common.Caching;
using Notes.Common.Messaging;
using Notes.Common.Security.Token;
using Notes.Data;
using Notes.Data.DataSeeder;
using Notes.Services.Identities.Commands;
using Notes.Services.Identities.Services;
using Notes.Services.Notes.Commands;
using Notes.WebAPI.Infrastructure.Caching;
using Notes.WebAPI.Infrastructure.Middleware;
using Notes.WebAPI.Modules.Identities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options => options.AddConsole());

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICache, MemoryCache>();
            
builder.Services.AddMvc()
    .ConfigureApiBehaviorOptions(o =>
    {
        o.SuppressMapClientErrors = true;
    });

builder.Services.AddDbContext<NotesContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    o => { o.MigrationsAssembly("Notes.WebAPI"); }));

builder.Services.AddAuthentication(sharedOptions =>
    {
        sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration.GetValue<string>("SymmetricSecurityKey") 
                    ?? throw new Exception("No symmetric security key"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Notes", Version = "v1" });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssembly(typeof(Program).Assembly);
    c.RegisterServicesFromAssembly(typeof(IdentityCreate).Assembly);
    c.RegisterServicesFromAssembly(typeof(NoteCreate).Assembly);
});

builder.Services.AddScoped<IMessageBroker, Notes.WebAPI.Infrastructure.Messaging.MediatR>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddSingleton<ITokenService>(o => new JwtTokenService(
    builder.Configuration.GetValue<string>("SymmetricSecurityKey") 
        ?? throw new Exception("No symmetric security key")));

var app = builder.Build();

UpdateDatabase(app, app.Environment, builder);

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Notes.WebAPI v1"));

    app.UseCors(o => o
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
}

//if (app.Environment.IsDevelopment())
  //  GenerateDevToken(app, builder);

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(o =>
{
    o.UseMiddleware<NotesExceptionHandlerMiddleware>();
});

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
return;


// methods
static void UpdateDatabase(IApplicationBuilder app, IWebHostEnvironment env, WebApplicationBuilder builder)
{
    if (env.EnvironmentName == "Testing")
        return;
    
    using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    using var notesContext = serviceScope.ServiceProvider.GetService<NotesContext>();

    if (!notesContext.Database.CanConnect())
        throw new Exception("Cannot connect to database.");
            
    notesContext.Database.Migrate();
    
    if (env.IsDevelopment())
        InitDataSeeder.SeedDataDev(notesContext, builder.Configuration.GetValue<string>("DevAcc"));
    else
        InitDataSeeder.SeedData(notesContext);
}

static void GenerateDevToken(IApplicationBuilder app, WebApplicationBuilder builder)
{
    using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    using var notesContext = serviceScope.ServiceProvider.GetService<NotesContext>();

    if (!notesContext.Database.CanConnect())
        throw new Exception("Cannot connect to database.");

    var email = builder.Configuration.GetValue<string>("DevAcc");
        
    var devUser = notesContext.Identities
        .FirstOrDefault(o => o.Email == email);

    if (devUser is null)
        throw new Exception("Cannot find devUser. " +
            "Check if DevAcc value in appsettings.Development.json has valid value and restart application");

    var payload = new Dictionary<string, string>
    {
        { "Id", devUser.Id.ToString() },
        { "Email", devUser.Email }
    };

    var tokenService = serviceScope.ServiceProvider.GetService<ITokenService>();

    var token = tokenService.IssueToken(TokenType.AccessToken, payload);

    var path = $"{Directory.GetCurrentDirectory()}\\development.jwt";
    
    File.WriteAllText(path, token);
}