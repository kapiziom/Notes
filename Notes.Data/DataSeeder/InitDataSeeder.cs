using Notes.Data.Entities;

namespace Notes.Data.DataSeeder;

public static class InitDataSeeder
{ 
    public static void SeedData(NotesContext context)
    {
        SeedTags(context);
    }
    
    public static void SeedDataDev(NotesContext context, string devAcc)
    {
        SeedTags(context);

        SeedUser(context, devAcc);
    }

    private static void SeedTags(NotesContext context)
    {
        if (context.Tags.Any())
            return;
        
        var tags = new List<TagEntity>
        {
            new()
            {
                Name = "PHONE",
                Regex = @"(?<!\w)(\(?(\+|00)?48\)?)?[ -]?\d{3}[ -]?\d{3}[ -]?\d{3}(?!\w)"
            },
            new()
            {
                Name = "EMAIL",
                Regex = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
            }
        };
        
        context.AddRange(tags);
        context.SaveChanges();
    }

    private static void SeedUser(NotesContext context, string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new Exception("DevAcc email is empty, add DevAcc value in appsettings.Development.json");
            
        if (context.Identities.Any(o => o.Email == email))
            return;

        var identity = new IdentityEntity
        {
            DateCreatedUtc = DateTime.UtcNow,
            Email = email,
            PasswordHash = $"{Guid.NewGuid()}",
            PasswordSalt = $"{Guid.NewGuid()}"
        };

        context.Add(identity);
        context.SaveChanges();
    }
}