using Notes.Data.Entities;

namespace Notes.Services.Tags.Dto;

public class TagDto
{
    public TagDto()
    {
        
    }

    public TagDto(TagEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Regex = entity.Regex;
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Regex { get; set; }
}