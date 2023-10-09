using System.Reflection;
using LearnNet_CartingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnNet_CartingService.Infrastructure.Data;

public partial class CartingServiceDbContext : DbContext
{
    public CartingServiceDbContext(DbContextOptions<CartingServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CartEntity> Carts => Set<CartEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

}
