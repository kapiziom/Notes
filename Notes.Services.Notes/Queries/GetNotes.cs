using Microsoft.EntityFrameworkCore;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Common.Paging;
using Notes.Data;
using Notes.Data.Entities;
using Notes.Services.Notes.Dto;

namespace Notes.Services.Notes.Queries;

public class GetNotes : IQuery<PageDto<NoteDto>>
{
    public GetNotes(int userId, PageInput pageInput, NoteFilterModel filterModel)
    {
        UserId = userId;
        PageInput = pageInput;
        FilterModel = filterModel;
    }

    public readonly int UserId;
    public readonly PageInput PageInput;
    public readonly NoteFilterModel FilterModel;
}

public class GetNotesHandler : IQueryHandler<GetNotes, PageDto<NoteDto>>
{
    private readonly NotesContext _context;

    public GetNotesHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<PageDto<NoteDto>> Handle(GetNotes query, CancellationToken ct = default)
    {
        var notesQuery = _context.Notes
            .AsNoTracking()
            .Include(o => o.NoteTags).ThenInclude(o => o.Tag)
            .AsQueryable();
        
        var isAsc = query.PageInput.SortOrder == SortOrder.Asc;

        notesQuery = query.PageInput.SortBy switch
        {
            nameof(NoteEntity.Id) => isAsc
                ? notesQuery.OrderBy(o => o.Id)
                : notesQuery.OrderByDescending(o => o.Id),
            nameof(NoteEntity.DateCreatedUtc) => isAsc
                ? notesQuery.OrderBy(o => o.DateCreatedUtc)
                : notesQuery.OrderByDescending(o => o.DateCreatedUtc),
            _ => isAsc
                ? notesQuery.OrderBy(o => o.DateCreatedUtc)
                : notesQuery.OrderByDescending(o => o.DateCreatedUtc),
        };
        
        var count = await notesQuery.CountAsync(ct);
        
        var notes = await notesQuery
            .Skip(query.PageInput.Offset)
            .Take(query.PageInput.PageSize)
            .Select(o => new
            {
                o.Id,
                o.DateCreatedUtc,
                o.DateModifiedUtc,
                o.Content,
                Tags = o.NoteTags.Select(x => x.Tag.Name)
            })
            .ToListAsync(ct);
            
        return new PageDto<NoteDto>
        {
            Offset = query.PageInput.Offset,
            PageSize = query.PageInput.PageSize,
            Total = count,
            Result = notes.Select(o => new NoteDto
            {
                Id = o.Id,
                DateCreatedUtc = o.DateCreatedUtc,
                DateModifiedUtc = o.DateModifiedUtc,
                Content = o.Content,
                Tags = o.Tags
            })
        };
    }
}