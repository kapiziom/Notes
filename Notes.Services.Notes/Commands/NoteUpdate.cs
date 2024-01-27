using Microsoft.EntityFrameworkCore;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Data.Entities;
using Notes.Services.Notes.Dto;
using Notes.Services.Notes.Exceptions;
using Notes.Services.Tags.Services;

namespace Notes.Services.Notes.Commands;

public class NoteUpdate : ICommand<NoteDetailsDto>
{
    public NoteUpdate(int id, int userId, string content)
    {
        Id = id;
        UserId = userId;
        Content = content;
    }

    public readonly int Id;
    public readonly int UserId;
    public readonly string Content;
}

public class NoteUpdateHandler : ICommandHandler<NoteUpdate, NoteDetailsDto>
{
    private readonly NotesContext _context;
    private readonly ITagService _tagService;

    public NoteUpdateHandler(NotesContext context, ITagService tagService)
    {
        _context = context;
        _tagService = tagService;
    }

    public async Task<NoteDetailsDto> Handle(NoteUpdate command, CancellationToken ct = default)
    {
        var note = await _context.Notes
            .Include(o => o.NoteTags)
            .FirstOrDefaultAsync(o => o.Id == command.Id
                && o.UserId == command.UserId, ct)
            ?? throw new NoteNotFoundException();
        
        var matchingTags = await _tagService.MatchingTags(command.Content, ct);

        note.Content = command.Content;
        note.DateModifiedUtc = DateTime.UtcNow;
        note.NoteTags = new List<NoteTagEntity>(
            matchingTags.Select(o => new NoteTagEntity
            {
                TagId = o.Id
            }));

        await _context.SaveChangesAsync(ct);

        return new NoteDetailsDto(note, matchingTags);
    }
}