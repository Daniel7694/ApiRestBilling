using ApiRestBilling.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRestBilling.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> ordersItem { get; set; }
        public DbSet<Product> products { get; set; }


    }
}
