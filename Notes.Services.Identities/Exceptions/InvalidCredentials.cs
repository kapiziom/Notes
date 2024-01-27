using Notes.Common.Exceptions;

namespace Notes.Services.Identities.Exceptions;

public class InvalidCredentials : ServiceException
{
    public InvalidCredentials()
        : base("Invalid credentials.")
    {
    }
}