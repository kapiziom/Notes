namespace Notes.Services.Identities.Services;

public interface IPasswordService
{
    void HashPassword(
        string password,
        out string passwordHash,
        out string passwordSalt);

    bool VerifyPassword(
        string password,
        string passwordHash,
        string passwordSalt);
}