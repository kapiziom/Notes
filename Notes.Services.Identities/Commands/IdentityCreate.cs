using Microsoft.EntityFrameworkCore;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Common.Security.Token;
using Notes.Data;
using Notes.Data.Entities;
using Notes.Services.Identities.Exceptions;
using Notes.Services.Identities.Services;

namespace Notes.Services.Identities.Commands;

public class IdentityCreate : ICommand<string>
{
    public IdentityCreate(string email, string password)
    {
        Email = email.Trim().ToLower();
        Password = password;
    }

    public readonly string Email;
    public readonly string Password;
}
    
public class IdentityCreateHandler : ICommandHandler<IdentityCreate, string>
{
    private readonly NotesContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public IdentityCreateHandler(
        NotesContext context, 
        IPasswordService passwordService, 
        ITokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(
        IdentityCreate command,
        CancellationToken ct = default)
    {
        if (await _context.Identities.AnyAsync(o => o.Email == command.Email, ct))
            throw new EmailAlreadyRegistered(command.Email);
            
        _passwordService.HashPassword(command.Password, out var passwordHash, out var passwordSalt);

        var identity = new IdentityEntity
        {
            DateCreatedUtc = DateTime.UtcNow,
            Email = command.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
        };

        _context.Add(identity);

        await _context.SaveChangesAsync(ct);

        return _tokenService.IssueToken(
            TokenType.AccessToken,
            new Dictionary<string, string>
            {
                { "Id", $"{identity.Id}" },
                { "Email", identity.Email }
            });
    }
}