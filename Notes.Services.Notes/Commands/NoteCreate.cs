using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Services.Notes.Dto;

namespace Notes.Services.Notes.Commands;

public class NoteCreate : ICommand<NoteDetailsDto>
{
    public NoteCreate(int userId, string content)
    {
        UserId = userId;
        Content = content;
    }

    public readonly int UserId;
    public readonly string Content;
}

public class NoteCreateHandler : ICommandHandler<NoteCreate, NoteDetailsDto>
{
    private readonly NotesContext _context;

    public NoteCreateHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<NoteDetailsDto> Handle(NoteCreate command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}