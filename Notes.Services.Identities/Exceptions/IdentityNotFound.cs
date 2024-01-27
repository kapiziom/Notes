using Notes.Common.Exceptions;

namespace Notes.Services.Identities.Exceptions;

public class IdentityNotFound : ServiceException
{
    public IdentityNotFound(string search) 
        : base($"Identity {search} not found.", ExceptionEnum.NotFound)
    {
    }
}