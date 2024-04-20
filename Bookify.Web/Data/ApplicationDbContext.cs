using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
          
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });

            //Create Sequence
            builder.HasSequence("SerialNumber")
                   .StartsAt(1000001);
            //Add Sequence to column
            builder.Entity<BookCopy>()
                   .Property(e => e.SerialNumber)
                   .HasDefaultValueSql("NEXT VALUE FOR SerialNumber");

            base.OnModelCreating(builder);
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategory { get; set; }
        public DbSet<BookCopy> BookCopy { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
