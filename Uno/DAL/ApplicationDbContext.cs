using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApplicationDbContext : DbContext
{
    // public DbSet<Contact> Contacts { get; set; }
    // public DbSet<ContactType> ContactTypes { get; set; }
    // public DbSet<Entry> Entries { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
}