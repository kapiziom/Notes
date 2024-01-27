namespace Notes.Services.Notes.Dto;

public class NoteFilterModel
{
    public IEnumerable<string> Tags { get; set; }
    public DateTime? DateCreatedUtcFrom { get; set; }
    public DateTime? DateCreatedUtcTo { get; set; }
}