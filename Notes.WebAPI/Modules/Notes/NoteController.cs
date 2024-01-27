using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Common.Messaging;
using Notes.Common.Paging;
using Notes.Services.Notes.Commands;
using Notes.Services.Notes.Dto;
using Notes.Services.Notes.Queries;

namespace Notes.WebAPI.Modules.Notes;

[Authorize]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly IMessageBroker _messageBroker;

    public NoteController(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> Get(
        [FromQuery] object filter,
        [FromQuery] PageInput pager,
        CancellationToken ct = default) =>
        Ok(await _messageBroker.SendQueryAsync(new GetNotes(pager), ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDetailsDto>> GetById(
        [FromRoute] int id,
        CancellationToken ct = default) =>
        await _messageBroker.SendQueryAsync(new GetNote(id), ct);

    [HttpPatch("{id}")]
    public async Task<ActionResult<NoteDetailsDto>> Update(
        [FromRoute] int id,
        [FromBody] NoteUpdateInput input,
        CancellationToken ct = default) => 
        await _messageBroker.SendCommandAsync(input.ToCommand(id), ct);

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id,
        CancellationToken ct = default)
    {
        await _messageBroker.SendCommandAsync(new NoteDelete(id), ct);

        return NoContent();
    }
}