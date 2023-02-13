using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace ZooIS.Models
{
    [Display(Name = "Пользователь")]
    public class User : IdentityUser, IEntity
    {
        public Guid? EmployeeGuid { get; set; }
        [Display(Name = "Работник")]
        public virtual Employee? Employee { get; set; }

        [Display(Name = "Роли")]
        public virtual HashSet<Role> Roles { get; set; } = new();

        [Display(Name ="Оповещения")]
        public HashSet<Alert> Alerts { get; set; } = new();
        [Display(Name = "Отчеты")]
        public HashSet<Report> Reports { get; set; } = new();

        public Employee AsEmployee() => Employee;

        public string Display { get => Employee?.Display ?? UserName; }

        public object Key { get => Id; }
    }

    [Display(Name = "Роль")]
    public class Role : IdentityRole, IEntity
    {
        [Display(Name = "Пользователи")]
        public virtual HashSet<User> Users { get; set; } = new();

        private string _Display;

        public string Display { get => _Display; set => _Display = value; }

        public object Key { get => Id; }
    }
}
