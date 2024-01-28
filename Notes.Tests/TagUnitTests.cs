using Microsoft.Extensions.DependencyInjection;
using Moq;
using Notes.Common.Messaging;
using Notes.Services.Tags.Dto;
using Notes.Services.Tags.Queries;
using Notes.Services.Tags.Services;
using Notes.Tests.Common;
using Notes.WebAPI.Modules.Tags;
using Xunit;

namespace Notes.Tests;

public class TagUnitTests
{
    [Fact]
    public async Task TestMatching_OneTag_ContentShouldMatch_MatchedTag()
    {
        // Arrange
        var getTags = new Mock<IMessageBroker>();
        getTags.Setup(x => x.SendQueryAsync(It.IsAny<GetTags>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<TagDto>
            {
                new()
                {
                    Id = 1,
                    Name = "EMAIL",
                    Regex = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
                }
            });

        var tagsService = new TagService(getTags.Object);
        var content = "example note test@email.com";
        
        // Act
        var matchingTags = await tagsService.MatchingTags(content);
        
        // Assert
        Assert.Contains(matchingTags, x => x.Name == "EMAIL");
        Assert.True(matchingTags.Count() == 1);
    }
    
    [Fact]
    public async Task TestMatching_OneTag_ContentShouldNotMatch_NoMatch()
    {
        // Arrange
        var getTags = new Mock<IMessageBroker>();
        getTags.Setup(x => x.SendQueryAsync(It.IsAny<GetTags>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<TagDto>
            {
                new()
                {
                    Id = 1,
                    Name = "EMAIL",
                    Regex = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
                }
            });

        var tagsService = new TagService(getTags.Object);
        var content = "example note +48123123123";
        
        // Act
        var matchingTags = await tagsService.MatchingTags(content);
        
        // Assert
        Assert.True(!matchingTags.Any());
    }
    
    [Fact]
    public async Task TestMatching_TwoTags_ContentShouldMatchBothTags_MatchedBoth()
    {
        // Arrange
        var getTags = new Mock<IMessageBroker>();
        getTags.Setup(x => x.SendQueryAsync(It.IsAny<GetTags>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<TagDto>
            {
                new()
                {
                    Id = 1,
                    Name = "EMAIL",
                    Regex = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
                },
                new()
                {
                    Id = 2,
                    Name = "PHONE",
                    Regex = @"(?<!\w)(\(?(\+|00)?48\)?)?[ -]?\d{3}[ -]?\d{3}[ -]?\d{3}(?!\w)"
                }
            });

        var tagsService = new TagService(getTags.Object);

        var content = "example note test@email.com +48123123123";
        
        // Act
        var matchingTags = await tagsService.MatchingTags(content);
        
        // Assert
        Assert.Contains(matchingTags, x => x.Name == "EMAIL");
        Assert.Contains(matchingTags, x => x.Name == "PHONE");
        Assert.True(matchingTags.Count() == 2);
    }
    
    [Fact]
    public async Task TestMatching_TwoTags_ContentShouldMatchOne_MatchedEmailTag()
    {
        // Arrange
        var getTags = new Mock<IMessageBroker>();
        getTags.Setup(x => x.SendQueryAsync(It.IsAny<GetTags>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<TagDto>
            {
                new()
                {
                    Id = 1,
                    Name = "EMAIL",
                    Regex = @"([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)"
                },
                new()
                {
                    Id = 2,
                    Name = "PHONE",
                    Regex = @"(?<!\w)(\(?(\+|00)?48\)?)?[ -]?\d{3}[ -]?\d{3}[ -]?\d{3}(?!\w)"
                }
            });
        
        var tagsService = new TagService(getTags.Object);

        var content = "example note test@email.com";
        
        // Act
        var matchingTags = await tagsService.MatchingTags(content);
        
        // Assert
        Assert.Contains(matchingTags, x => x.Name == "EMAIL");
        Assert.True(matchingTags.Count() == 1);
    }
}