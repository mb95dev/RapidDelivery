using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Core.Entities;

namespace Orders.Infrastructure.Contexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext()
    {

    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

    }

}



