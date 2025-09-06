using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Services.Description;

namespace ChhayaNirh.Models
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor specifies the connection string name from Web.config
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        // Add your tables here as DbSet properties
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        //public DbSet<Product> Products { get; set; }

        // Example:
        // public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove default cascade delete convention
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Configure Comment → User (required, no cascade delete)
            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false); // ✅ Prevents multiple cascade paths

            // Configure Comment → Post (required, cascade delete enabled)
            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .WillCascadeOnDelete(true); // ✅ Automatically deletes comments when post is deleted
                                            // Configure Like → User (required, no cascade delete)
            modelBuilder.Entity<Like>()
                .HasRequired(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .WillCascadeOnDelete(false);

            // Configure Like → Post (required, cascade delete enabled)
            modelBuilder.Entity<Like>()
                .HasRequired(l => l.Post)
                .WithMany()
                .HasForeignKey(l => l.PostId)
                .WillCascadeOnDelete(true);

            // Ensure unique combination of UserId and PostId for Likes
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
