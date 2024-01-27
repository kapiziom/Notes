using Microsoft.EntityFrameworkCore;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Common.Security.Token;
using Notes.Data;
using Notes.Services.Identities.Exceptions;
using Notes.Services.Identities.Services;

namespace Notes.Services.Identities.Commands;

public class IdentityAuthenticate : ICommand<string>
{
    public IdentityAuthenticate(
        string email,
        string password)
    {
        Email = email.ToLower();
        Password = password;
    }

    public readonly string Email;
    public readonly string Password;
}
    
public class IdentityAuthenticateHandler : ICommandHandler<IdentityAuthenticate, string>
{
    private readonly NotesContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public IdentityAuthenticateHandler(NotesContext context, 
        IPasswordService passwordService, ITokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(
        IdentityAuthenticate command,
        CancellationToken ct = default)
    {
        var identity = await _context.Identities
            .AsNoTracking()
            .Where(o => o.Email == command.Email)
            .SingleOrDefaultAsync(ct) ?? throw new IdentityNotFound(command.Email);

        if (!_passwordService.VerifyPassword(command.Password, identity.PasswordHash, identity.PasswordSalt))
            throw new InvalidCredentials();

        return _tokenService.IssueToken(
            TokenType.AuthorizationCode,
            new Dictionary<string, string>
            {
                { "Id", $"{identity.Id}" },
                { "Email", identity.Email }
            });
    }
}