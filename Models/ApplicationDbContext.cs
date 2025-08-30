using System.Data.Entity;

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
        //public DbSet<Product> Products { get; set; }

        // Example:
        // public DbSet<Order> Orders { get; set; }
    }
}
