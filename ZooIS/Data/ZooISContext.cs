using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ZooIS.Models;

namespace ZooIS.Data
{
    public class ZooISContext : IdentityDbContext<User, Role, string>
    {
        public ZooISContext(DbContextOptions<ZooISContext> options)
            : base(options)
        {
        }
        public DbSet<Taxon> Taxons { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Animal>().HasCheckConstraint("BirthDate_NoFuture", "BirthDate <= CURRENT_TIMESTAMP");
            builder.Entity<IdentityUserRole<string>>()
                .ToTable("AspNetUserRoles");
        }
    }
}
