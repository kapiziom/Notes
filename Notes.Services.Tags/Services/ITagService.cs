using Notes.Services.Tags.Dto;

namespace Notes.Services.Tags.Services;

public interface ITagService
{
    Task<IEnumerable<TagDto>> MatchingTags(string content, CancellationToken ct = default);
}