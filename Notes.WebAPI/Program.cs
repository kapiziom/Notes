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
using Notes.Services.Tags.Dto;
using Notes.Services.Tags.Services;
using Notes.WebAPI.Infrastructure.Caching;
using Notes.WebAPI.Infrastructure.Middleware;
using Notes.WebAPI.Modules.Identities;
using Notes.WebAPI.Modules.Tags;

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

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
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
    c.RegisterServicesFromAssembly(typeof(TagDto).Assembly);
});

builder.Services.AddScoped<IMessageBroker, Notes.WebAPI.Infrastructure.Messaging.MediatR>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddSingleton<ITokenService>(o => new JwtTokenService(
    builder.Configuration.GetValue<string>("SymmetricSecurityKey") 
        ?? throw new Exception("No symmetric security key")));

var app = builder.Build();

UpdateDatabase(app, app.Environment);

SeedData(app, app.Environment, builder);

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<NotesExceptionHandlerMiddleware>();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
return;


// methods
static void UpdateDatabase(IApplicationBuilder app, IWebHostEnvironment env)
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
}

static void SeedData(IApplicationBuilder app, IWebHostEnvironment env, WebApplicationBuilder builder)
{
    using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    using var notesContext = serviceScope.ServiceProvider.GetService<NotesContext>();

    if (!notesContext.Database.CanConnect())
        throw new Exception("Cannot connect to database.");
    
    if (env.IsDevelopment() || env.EnvironmentName == "Testing")
        InitDataSeeder.SeedDataDev(notesContext, builder.Configuration.GetValue<string>("DevAcc"));
    else
        InitDataSeeder.SeedData(notesContext);
}