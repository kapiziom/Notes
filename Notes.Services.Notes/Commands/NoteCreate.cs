using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Data.Entities;
using Notes.Services.Notes.Dto;
using Notes.Services.Tags.Services;

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
    private readonly ITagService _tagService;

    public NoteCreateHandler(NotesContext context, ITagService tagService)
    {
        _context = context;
        _tagService = tagService;
    }

    public async Task<NoteDetailsDto> Handle(NoteCreate command, CancellationToken ct = default)
    {
        var matchingTags = await _tagService.MatchingTags(command.Content, ct);
        
        var note = new NoteEntity
        {
            DateCreatedUtc = DateTime.UtcNow,
            Content = command.Content,
            UserId = command.UserId,
            NoteTags = new List<NoteTagEntity>(
                matchingTags.Select(o => new NoteTagEntity
                {
                    TagId = o.Id
                }))
        };

        _context.Add(note);

        await _context.SaveChangesAsync(ct);
        
        return new NoteDetailsDto(note, matchingTags);
    }
}