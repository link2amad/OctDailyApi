using Microsoft.EntityFrameworkCore;
using OctDailyApi.Models;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

    // Products table in the database
    public DbSet<Product> Products { get; set; }
}
