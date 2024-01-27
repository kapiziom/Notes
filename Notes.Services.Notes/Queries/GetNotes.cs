﻿using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Common.Paging;
using Notes.Data;
using Notes.Services.Notes.Dto;

namespace Notes.Services.Notes.Queries;

public class GetNotes : IQuery<IEnumerable<NoteDto>>
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

public class GetNotesHandler : IQueryHandler<GetNotes, IEnumerable<NoteDto>>
{
    private readonly NotesContext _context;

    public GetNotesHandler(NotesContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NoteDto>> Handle(GetNotes query, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}