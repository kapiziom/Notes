using Microsoft.AspNetCore.Mvc;
using Notes.Common.Messaging;

namespace Notes.WebAPI.Modules.Identities;

[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMessageBroker _messageBroker;

    public IdentityController(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }
    
    [HttpPost("Authenticate")] 
    public async Task<ActionResult<string>> Login(
        [FromBody] IdentityLoginInput model, 
        CancellationToken ct = default) =>
        await _messageBroker.SendCommandAsync(model.ToCommand(), ct);
    
    [HttpPost("Create")]
    public async Task<ActionResult<string>> Create(
        [FromBody] IdentityRegisterInput model,
        CancellationToken ct = default) =>
        Created(string.Empty, await _messageBroker.SendCommandAsync(model.ToCommand(), ct));
}