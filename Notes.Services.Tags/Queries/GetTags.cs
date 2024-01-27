using Microsoft.EntityFrameworkCore;
using Notes.Common.Caching;
using Notes.Common.Messaging.Handlers;
using Notes.Common.Messaging.Messages;
using Notes.Data;
using Notes.Services.Tags.Dto;

namespace Notes.Services.Tags.Queries;

public class GetTags : IQuery<IEnumerable<TagDto>>
{
    
}

public class GetTagsHandler : IQueryHandler<GetTags, IEnumerable<TagDto>>
{
    private readonly NotesContext _context;
    private readonly ICache _cache;

    public GetTagsHandler(NotesContext context, ICache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<TagDto>> Handle(GetTags query, CancellationToken ct = default)
    {
        var tags = await _cache.GetAsync<List<TagDto>>(nameof(GetTags), ct);

        if (tags is not null && tags.Count != 0) 
            return tags;
        
        tags = await _context.Tags.AsNoTracking()
            .Select(o => new TagDto(o))
            .ToListAsync(ct);

        await _cache.SetAsync(nameof(GetTags), tags, new CacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        }, ct);

        return tags;
    }
}