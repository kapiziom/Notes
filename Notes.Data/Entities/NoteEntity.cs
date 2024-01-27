using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Entities;

public class NoteEntity
{
    public int Id { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Content { get; set; }
    
    public int UserId { get; set; }
    public IdentityEntity User { get; set; }
    
    public virtual ICollection<NoteTagEntity> NoteTags { get; set; }
}

public class NoteMap : IEntityTypeConfiguration<NoteEntity>
{
    public void Configure(EntityTypeBuilder<NoteEntity> builder)
    {
        builder.ToTable("Notes");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Content)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany(t => t.Notes)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}