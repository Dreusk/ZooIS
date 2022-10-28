using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ZooIS.Models;

namespace ZooIS.Data
{
    public class ZooISContext : IdentityDbContext
    {
        public ZooISContext(DbContextOptions<ZooISContext> options)
            : base(options)
        {
        }
        public DbSet<Species> Species { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public DbSet<Employee> Employees { get; set; }
    }

    [Display(Name = "Пользователь")]
    public class User : IdentityUser
    {
        protected Guid? EmployeeGuid { get; set; }
        [Display(Name = "Работник")]
        protected virtual Employee? Employee { get; set; }

        public Employee AsEmployee() => Employee;
    }
}
