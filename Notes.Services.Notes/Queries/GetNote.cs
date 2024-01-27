using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Services.Notes.Dto;

namespace Notes.Services.Notes.Queries;

public class GetNote : IQuery<NoteDetailsDto>
{
    public GetNote(int id, int userId)
    {
        Id = id;
        UserId = userId;
    }

    public readonly int Id;
    public readonly int UserId;
}

public class GetNoteHandler : IQueryHandler<GetNote, NoteDetailsDto>
{
    private readonly NotesContext _context;

    public GetNoteHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<NoteDetailsDto> Handle(GetNote query, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}