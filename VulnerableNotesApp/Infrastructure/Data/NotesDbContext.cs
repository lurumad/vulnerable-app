using Microsoft.EntityFrameworkCore;
using VulnerableNotesApp.Model;

namespace VulnerableNotesApp.Infrastructure.Data
{
    public class NotesDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
        {

        }
    }
}
