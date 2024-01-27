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
    public DbSet<NoteTagEntity> NoteTags { get; set; }
    public DbSet<TagEntity> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new IdentityMap());
        modelBuilder.ApplyConfiguration(new NoteMap());
        modelBuilder.ApplyConfiguration(new NoteTagMap());
        modelBuilder.ApplyConfiguration(new TagMap());
    }
}