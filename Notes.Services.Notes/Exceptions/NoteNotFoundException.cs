using Notes.Common.Exceptions;

namespace Notes.Services.Notes.Exceptions;

public class NoteNotFoundException : ServiceException
{
    public NoteNotFoundException() : base(string.Empty, ExceptionEnum.NotFound)
    {
    }
}