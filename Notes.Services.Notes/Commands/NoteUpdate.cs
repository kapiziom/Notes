using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Services.Notes.Dto;

namespace Notes.Services.Notes.Commands;

public class NoteUpdate : ICommand<NoteDetailsDto>
{
    public NoteUpdate(int id, string content)
    {
        Id = id;
        Content = content;
    }

    public readonly int Id;
    public readonly string Content;
}

public class NoteUpdateHandler : ICommandHandler<NoteUpdate, NoteDetailsDto>
{
    private readonly NotesContext _context;

    public NoteUpdateHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<NoteDetailsDto> Handle(NoteUpdate command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}