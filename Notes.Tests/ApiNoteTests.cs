using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Notes.Tests.Common;
using Notes.WebAPI.Modules.Identities;
using Notes.WebAPI.Modules.Notes;
using Xunit;

namespace Notes.Tests;

public class ApiNoteTests
{
    private readonly TestApplication _application = new ();
    private readonly HttpClient _httpClient;

    public ApiNoteTests()
    {
        _httpClient = _application.CreateClient();
    }
    
    // tags and userId:1 inserted to inmemory db InitDataSeeder.SeedTags

    [Fact]
    public async Task CreateNote_RegisterUserAndCreateCorrectNote_AllSuccessResponses()
    {
        // Arrange
        var register = new IdentityRegisterInput
        {
            Email = $"{Guid.NewGuid()}@email.com",
            Password = $"{Guid.NewGuid()}"
        };
        var jsonContent = JsonSerializer.Serialize(register);
        
        var noteCreate = new NoteCreateInput
        {
            Content = "test example note"
        };
        var noteJsonContent = JsonSerializer.Serialize(noteCreate);

        // Act
        var registerResponse = await _httpClient.PostAsync("api/Identity/Create",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        
        var token = await registerResponse.Content.ReadAsStringAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var createNoteResponse = await _httpClient.PostAsync("api/Note",
            new StringContent(noteJsonContent, Encoding.UTF8, "application/json"));
        
        Assert.True(registerResponse.IsSuccessStatusCode);
        Assert.True(!string.IsNullOrEmpty(token));
        Assert.True(createNoteResponse.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task CreateNote_RegisterUserAndCreateWithTooLongContent_Returns400()
    {
        // Arrange
        var register = new IdentityRegisterInput
        {
            Email = $"{Guid.NewGuid()}@email.com",
            Password = $"{Guid.NewGuid()}"
        };
        var jsonContent = JsonSerializer.Serialize(register);

        var attribute = (MaxLengthAttribute) GetAttribute(
            typeof(NoteInput), typeof(MaxLengthAttribute), nameof(NoteInput.Content));
        
        var noteCreate = new NoteCreateInput
        {
            Content = GenerateRandomString(attribute.Length + 1)
        };
        var noteJsonContent = JsonSerializer.Serialize(noteCreate);

        // Act
        var registerResponse = await _httpClient.PostAsync("api/Identity/Create",
            new StringContent(jsonContent, Encoding.UTF8, "application/json"));
        
        var token = await registerResponse.Content.ReadAsStringAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var createNoteResponse = await _httpClient.PostAsync("api/Note",
            new StringContent(noteJsonContent, Encoding.UTF8, "application/json"));
        
        Assert.True(registerResponse.IsSuccessStatusCode);
        Assert.True(!string.IsNullOrEmpty(token));
        Assert.Equal(HttpStatusCode.BadRequest, createNoteResponse.StatusCode);
    }


    [Fact]
    public async Task CreateNote_CreateAsUnauthorized_Returns401()
    {
        // Arrange
        var noteCreate = new NoteCreateInput
        {
            Content = "test example note"
        };
        var noteJsonContent = JsonSerializer.Serialize(noteCreate);
        
        // Act
        var createNoteResponse = await _httpClient.PostAsync("api/Note",
            new StringContent(noteJsonContent, Encoding.UTF8, "application/json"));
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, createNoteResponse.StatusCode);
    }
    
    // help methods
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        Random random = new Random();
        StringBuilder stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            stringBuilder.Append(chars[index]);
        }

        return stringBuilder.ToString();
    }
    
    private Attribute GetAttribute(Type type, Type attributeType, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName);
        var attribute = propertyInfo?.GetCustomAttributes(attributeType, false)
            .FirstOrDefault() as Attribute;

        return attribute;
    }
}