using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace IT_Company.Models
{
    public class EmployeePositionContext : DbContext
    {
        private static DbContextOptions<EmployeePositionContext> dbOptions;
        static EmployeePositionContext()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("DB.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<EmployeePositionContext>();
            dbOptions = optionsBuilder.UseLazyLoadingProxies().UseSqlServer(connectionString).Options;
        }

        public EmployeePositionContext() : base(dbOptions)
        {}
      
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Positions");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LastName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.FirstName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Age)
                      .IsRequired();

                entity.HasOne(e => e.Position)
                      .WithMany(p => p.Employees)
                      .HasForeignKey(e => e.PositionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}