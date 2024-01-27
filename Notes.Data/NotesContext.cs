using Microsoft.EntityFrameworkCore;
using Notes.Data.Entities;

namespace Notes.Data;

public class NotesContext : DbContext
{
    public NotesContext(DbContextOptions<NotesContext> options) : base(options)
    {
    }
    
    public DbSet<IdentityEntity> Identities { get; set; }
    public DbSet<NoteEntity> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new IdentityMap());
        modelBuilder.ApplyConfiguration(new NoteMap());
    }
}