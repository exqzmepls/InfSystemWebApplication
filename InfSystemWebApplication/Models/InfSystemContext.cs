using System.Data.Entity;

namespace InfSystemWebApplication.Models
{
    public class InfSystemContext : DbContext
    {
        public InfSystemContext() : base() { }

        public InfSystemContext(string connectionString) : base(connectionString) { }

        public DbSet<ProductClass> ProductClasses { get; set; }
        public DbSet<ProductSubclass> ProductSubClasses { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductSubgroup> ProductSubGroups { get; set; }
        public DbSet<ProductKind> ProductKinds { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<SoldProduct> SoldProducts { get; set; }
    }
}