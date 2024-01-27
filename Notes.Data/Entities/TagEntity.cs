using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Entities;

public class TagEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Regex { get; set; }
    
    public virtual ICollection<NoteTagEntity> NoteTags { get; set; }
}


public class TagMap : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.ToTable("Tags");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.Regex)
            .HasMaxLength(1024)
            .IsRequired();
    }
}