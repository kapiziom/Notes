using Notes.Common.Exceptions;

namespace Notes.Services.Identities.Exceptions;

public class InvalidPasswordFormat : ServiceException
{
    public InvalidPasswordFormat()
        : base($"Invalid password format.")
    {
    }
}