using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;

namespace Notes.Services.Notes.Commands;

public class NoteDelete : ICommand<bool>
{
    public NoteDelete(int id, int userId)
    {
        Id = id;
        UserId = userId;
    }

    public readonly int Id;
    public readonly int UserId;
}

public class NoteDeleteHandler : ICommandHandler<NoteDelete, bool>
{
    private readonly NotesContext _context;

    public NoteDeleteHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(NoteDelete command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}