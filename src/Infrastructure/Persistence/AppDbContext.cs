using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BookRow> Books => Set<BookRow>();
    public DbSet<CopyRow> Copies => Set<CopyRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var book = modelBuilder.Entity<BookRow>();
        book.HasKey(b => b.Id);
        book.Property(b => b.Id).HasMaxLength(32);
        book.Property(b => b.Isbn).IsRequired().HasMaxLength(32);
        book.Property(b => b.Title).IsRequired().HasMaxLength(200);
        book.HasIndex(b => b.Isbn).IsUnique();

 
        book.HasMany(b => b.Copies)  //1:N
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade); 

        var copy = modelBuilder.Entity<CopyRow>();
        copy.HasKey(c => c.Id);
        copy.Property(c => c.IsIssued).IsRequired();
    }
}