using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Entities;

public class NoteTagEntity
{
    public int NoteId { get; set; }
    public NoteEntity Note { get; set; }
    
    public int TagId { get; set; }
    public TagEntity Tag { get; set; }
}


public class NoteTagMap : IEntityTypeConfiguration<NoteTagEntity>
{
    public void Configure(EntityTypeBuilder<NoteTagEntity> builder)
    {
        builder.ToTable("NoteTags");
        
        builder.HasKey(t => new { t.NoteId, t.TagId });

        builder.HasOne(t => t.Note)
            .WithMany(t => t.NoteTags)
            .HasForeignKey(t => t.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Tag)
            .WithMany(t => t.NoteTags)
            .HasForeignKey(t => t.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}