using Microsoft.Extensions.DependencyInjection;
using Notes.Common.Messaging;
using Notes.Data;
using Notes.Services.Notes.Commands;
using Notes.Services.Notes.Exceptions;
using Notes.Services.Notes.Queries;
using Notes.Services.Tags.Queries;
using Notes.Tests.Common;
using Xunit;

namespace Notes.Tests;

public class NotesTests
{
    private readonly TestApplication _application = new ();
    
    // tags and userId:1 inserted to inmemory db InitDataSeeder.SeedTags
    
    [Fact]
    public async Task CreateNote_CorrectAddNoTags_Ok()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note";
        
        // Act
        var result = await messageBroker.SendCommandAsync(new NoteCreate(1, content));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Content, content);
        Assert.True(!result.Tags.Any());
    }
    
    [Fact]
    public async Task CreateNote_CorrectAddPhoneTag_Ok()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note +48123321123 example note";
        
        // Act
        var result = await messageBroker.SendCommandAsync(new NoteCreate(1, content));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Content, content);
        Assert.Contains(result.Tags, x => x.Equals("PHONE", StringComparison.InvariantCultureIgnoreCase));
        Assert.DoesNotContain(result.Tags, x => x.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase));
    }
    
    [Fact]
    public async Task CreateNote_CorrectAddEmailTag_Ok()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note test@email.com example note";
        
        // Act
        var result = await messageBroker.SendCommandAsync(new NoteCreate(1, content));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Content, content);
        Assert.Contains(result.Tags, x => x.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase));
        Assert.DoesNotContain(result.Tags, x => x.Equals("PHONE", StringComparison.InvariantCultureIgnoreCase));
    }
    
    [Fact]
    public async Task CreateNote_CorrectAddEmailPhoneTags_Ok()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note test@email.com example note";
        
        // Act
        var result = await messageBroker.SendCommandAsync(new NoteCreate(1, content));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Content, content);
        Assert.Contains(result.Tags, x => x.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase));
        Assert.DoesNotContain(result.Tags, x => x.Equals("PHONE", StringComparison.InvariantCultureIgnoreCase));
    }
    
    [Fact]
    public async Task DeleteNote_CorrectDelete_CheckByIdReturns404()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note test@email.com example note";
        var note = await messageBroker.SendCommandAsync(new NoteCreate(1, content));
        
        // Act
        var result = await messageBroker.SendCommandAsync(new NoteDelete(note.Id, 1));

        // Assert
        Assert.NotNull(note);
        Assert.True(result);
        
        await Assert.ThrowsAsync<NoteNotFoundException>(
            async () => await messageBroker.SendQueryAsync(new GetNote(note.Id, 1)));
    }
    
    [Fact]
    public async Task DeleteNote_TryDeleteNotExisting_404()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        // Act & Assert
        await Assert.ThrowsAsync<NoteNotFoundException>(
            async () => await messageBroker.SendCommandAsync(new NoteDelete(99999, 1)));
    }
    
    [Fact]
    public async Task DeleteNote_TryDeleteNotOwningItem_404()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note";
        var note = await messageBroker.SendCommandAsync(new NoteCreate(1, content));
        
        // Act & Assert
        Assert.NotNull(note);
        await Assert.ThrowsAsync<NoteNotFoundException>(
            async () => await messageBroker.SendCommandAsync(new NoteDelete(note.Id, 2)));
    }

    [Fact]
    public async Task UpdateNote_ChangeTagPhoneToEmail_Ok()
    {
        // Arrange
        await using var scope = _application.Services.CreateAsyncScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var content = "example note +48123321123";
        var createdNote = await messageBroker.SendCommandAsync(new NoteCreate(1, content));
        
        content = "example note test@email.com";
        // Act
        var updatedNote = await messageBroker.SendCommandAsync(new NoteUpdate(createdNote.Id, 1, content));
        
        Assert.NotNull(createdNote);
        Assert.NotNull(updatedNote);
        Assert.Equal(updatedNote.Content, content);
        Assert.Contains(createdNote.Tags, x => x.Equals("PHONE", StringComparison.InvariantCultureIgnoreCase));
        Assert.DoesNotContain(createdNote.Tags, x => x.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase));
        
        Assert.Contains(updatedNote.Tags, x => x.Equals("EMAIL", StringComparison.InvariantCultureIgnoreCase));
        Assert.DoesNotContain(updatedNote.Tags, x => x.Equals("PHONE", StringComparison.InvariantCultureIgnoreCase));
    }
}