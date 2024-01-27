using Notes.Common.Exceptions;

namespace Notes.Services.Identities.Exceptions;

public class EmailAlreadyRegistered : ServiceException
{
    public EmailAlreadyRegistered(string email)
        : base($"Identity with email: {email} already registered")
    {
    }
}