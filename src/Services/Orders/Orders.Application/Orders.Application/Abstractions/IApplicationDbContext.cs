using Microsoft.EntityFrameworkCore;
using Orders.Core.Entities;

namespace Orders.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Order> Orders { get; }
}

