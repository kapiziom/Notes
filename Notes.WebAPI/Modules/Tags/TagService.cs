using System.Text.RegularExpressions;
using Notes.Common.Messaging;
using Notes.Services.Tags.Dto;
using Notes.Services.Tags.Queries;
using Notes.Services.Tags.Services;

namespace Notes.WebAPI.Modules.Tags;

public class TagService : ITagService
{
    private readonly IMessageBroker _messageBroker;

    public TagService(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task<IEnumerable<TagDto>> MatchingTags(string content, CancellationToken ct = default)
    {
        var tags = await _messageBroker.SendQueryAsync(new GetTags(), ct);

        return tags.Where(tag => Regex.IsMatch(content, tag.Regex)).ToList();
    }
}