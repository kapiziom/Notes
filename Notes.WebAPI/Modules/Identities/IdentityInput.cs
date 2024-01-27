using System.ComponentModel.DataAnnotations;
using Notes.Services.Identities.Commands;

namespace Notes.WebAPI.Modules.Identities;

public abstract class IdentityInput
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}

public class IdentityRegisterInput : IdentityInput
{
    public IdentityCreate ToCommand() =>
        new (Email, Password);
}

public class IdentityLoginInput : IdentityInput
{
    public IdentityAuthenticate ToCommand() =>
        new (Email, Password);
}