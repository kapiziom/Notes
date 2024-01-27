using Notes.Common.Exceptions;

namespace Notes.Services.Identities.Exceptions;

public class InvalidPasswordLength : ServiceException
{
    public InvalidPasswordLength(int minimumLength, int maximumLength)
        : base($"Password length must be between {minimumLength} and {maximumLength} characters.")
    {
    }
}