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
                Regex = @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\W*\d\W*\d\W*\d\W*\d\W*\d\W*\d\W*\d\W*\d\W*(\d{1,2})"
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