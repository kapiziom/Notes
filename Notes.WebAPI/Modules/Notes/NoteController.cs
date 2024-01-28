using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Common.Messaging;
using Notes.Common.Paging;
using Notes.Services.Notes.Commands;
using Notes.Services.Notes.Dto;
using Notes.Services.Notes.Queries;
using Notes.WebAPI.Infrastructure.Attributes;

namespace Notes.WebAPI.Modules.Notes;

[Authorize]
[ValidateModel]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly IMessageBroker _messageBroker;

    public NoteController(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    [HttpPost]
    public async Task<ActionResult<NoteDetailsDto>> Create(
        [FromBody] NoteCreateInput input,
        CancellationToken ct = default) => 
        await _messageBroker.SendCommandAsync(input.ToCommand(UserId()), ct);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> Get(
        [FromQuery] NoteFilterModel filter,
        [FromQuery] PageInput pager,
        CancellationToken ct = default) =>
        Ok(await _messageBroker.SendQueryAsync(new GetNotes(UserId(), pager, filter), ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDetailsDto>> GetById(
        [FromRoute] int id,
        CancellationToken ct = default) =>
        await _messageBroker.SendQueryAsync(new GetNote(id, UserId()), ct);

    [HttpPatch("{id}")]
    public async Task<ActionResult<NoteDetailsDto>> Update(
        [FromRoute] int id,
        [FromBody] NoteUpdateInput input,
        CancellationToken ct = default) => 
        await _messageBroker.SendCommandAsync(input.ToCommand(id, UserId()), ct);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        await _messageBroker.SendCommandAsync(new NoteDelete(id, UserId()), ct);

        return NoContent();
    }

    private int UserId() => int.Parse(GetValueByClaimType("Id"));
        
    private string GetValueByClaimType(string claimType) =>
        User.Claims.SingleOrDefault(o => o.Type.Equals(claimType))?.Value ?? string.Empty;
}