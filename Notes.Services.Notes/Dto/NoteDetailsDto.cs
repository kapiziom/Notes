using Notes.Data.Entities;
using Notes.Services.Tags.Dto;

namespace Notes.Services.Notes.Dto;

public class NoteDetailsDto
{
    public NoteDetailsDto()
    {
        
    }

    public NoteDetailsDto(NoteEntity entity)
    {
        Id = entity.Id;
        DateCreatedUtc = entity.DateCreatedUtc;
        DateModifiedUtc = entity.DateModifiedUtc;
        Content = entity.Content;
        Tags = entity.NoteTags.Select(o => o.Tag.Name);
    }

    public NoteDetailsDto(NoteEntity entity, IEnumerable<TagDto> tags)
    {
        Id = entity.Id;
        DateCreatedUtc = entity.DateCreatedUtc;
        DateModifiedUtc = entity.DateModifiedUtc;
        Content = entity.Content;
        Tags = tags.Select(o => o.Name);
    }
    
    public int Id { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Content { get; set; }
    public IEnumerable<string> Tags { get; set; }
}