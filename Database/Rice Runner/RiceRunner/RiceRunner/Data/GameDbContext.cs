using RiceRunner.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace RiceRunner.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<InventoryItem> Inventory { get; set; }

        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Score)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(u => u.Rice)
                .HasDefaultValue(0);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.Quantity)
                .HasDefaultValue(1);
        }
    }
}
