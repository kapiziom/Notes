using Notes.Data.Entities;

namespace Notes.Services.Notes.Dto;

public class NoteDto
{
    public NoteDto()
    {
        
    }
    
    public int Id { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Content { get; set; }
    public IEnumerable<string> Tags { get; set; }
}