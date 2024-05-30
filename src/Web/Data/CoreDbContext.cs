using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SomeBasicEFApp.Web.Entities;
using SomeBasicEFApp.Web.ValueTypes;

namespace SomeBasicEFApp.Web.Data;

public class CoreDbContext : IdentityDbContext<ApplicationUser>
{
    public CoreDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>(entity =>
        {
            entity.Property(o => o.Id);
            entity
                .OwnsOne(o => o.Type,
                    t=>t.Property(pt=>pt.Type).HasColumnName("ProductType"))
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.HasMany(p => p.Orders)
                .WithMany(o => o.Products)
                .UsingEntity<ProductOrder>();

        });
        builder.Entity<Customer>(entity =>
        {
            entity.Property(o => o.Id);
        });
        builder.Entity<Order>(entity =>
        {
            entity.Property(o => o.Id);
        });
        base.OnModelCreating(builder);
    }

    public DbSet<Customer> Customers { get; init; }
    public DbSet<Order> Orders { get; init;  }
    public DbSet<Product> Products { get; init; }

    public DbSet<ProductOrder> ProductOrders { get; init; }

    public Customer? GetCustomer(CustomerId customerId) =>
        Customers.SingleOrDefault(customer => customer.Id == customerId.Value);

    public Product? GetProduct(ProductId productId) =>
        Products.SingleOrDefault(product => product.Id == productId.Value);

    public Order? GetOrder(OrderId orderId) =>
        Orders.SingleOrDefault(order => order.Id == orderId.Value);
    public async Task<Customer?> GetCustomerAsync(CustomerId customerId) =>
        await Customers.SingleOrDefaultAsync(customer => customer.Id == customerId.Value);

    public async Task<Product?> GetProductAsync(ProductId productId) =>
        await Products.SingleOrDefaultAsync(product => product.Id == productId.Value);

    public async Task<Order?> GetOrderAsync(OrderId orderId) =>
        await Orders.SingleOrDefaultAsync(order => order.Id == orderId.Value);

}
