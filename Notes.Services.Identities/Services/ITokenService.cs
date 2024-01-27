using Notes.Common.Security.Token;

namespace Notes.Services.Identities.Services;

public interface ITokenService
{
    string IssueToken(
        TokenType tokenType,
        Dictionary<string, string> payload);
}