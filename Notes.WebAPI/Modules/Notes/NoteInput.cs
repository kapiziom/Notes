using System.ComponentModel.DataAnnotations;
using Notes.Services.Notes.Commands;

namespace Notes.WebAPI.Modules.Notes;

public abstract class NoteInput
{
    [Required]
    [MaxLength(512)]
    public string Content { get; set; }
}

public class NoteCreateInput : NoteInput
{
    public NoteCreate ToCommand() =>
        new (Content);
}

public class NoteUpdateInput : NoteInput
{
    public NoteUpdate ToCommand(int id) =>
        new (id, Content);
}