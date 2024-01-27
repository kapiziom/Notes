using Microsoft.EntityFrameworkCore;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Services.Notes.Dto;
using Notes.Services.Notes.Exceptions;

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
        var note = await _context.Notes
            .AsNoTracking()
            .Include(o => o.NoteTags).ThenInclude(o => o.Tag)
            .FirstOrDefaultAsync(o => o.Id == query.Id
                && o.UserId == query.UserId, ct)
            ?? throw new NoteNotFoundException();

        return new NoteDetailsDto(note);
    }
}