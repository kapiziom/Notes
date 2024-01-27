using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Entities;

public class IdentityEntity
{
    public long Id { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}

public class IdentityMap : IEntityTypeConfiguration<IdentityEntity>
{
    public void Configure(EntityTypeBuilder<IdentityEntity> builder)
    {
        builder.ToTable("Identities");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(t => t.PasswordHash)
            .IsRequired();

        builder.Property(t => t.PasswordSalt)
            .IsRequired();
    }
}