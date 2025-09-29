using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WiproPOC_OrderApi.Model;

namespace WiproPOC_OrderApi.DAL
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
