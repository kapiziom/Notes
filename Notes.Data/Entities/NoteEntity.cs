using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Entities;

[Table("Notes")]
public class NoteEntity
{
    public int Id { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Content { get; set; }
}

public class NoteMap : IEntityTypeConfiguration<NoteEntity>
{
    public void Configure(EntityTypeBuilder<NoteEntity> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Content)
            .HasMaxLength(512)
            .IsRequired();
    }
}