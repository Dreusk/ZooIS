using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;
using OfficeOpenXml.Export.HtmlExport;

namespace ZooIS.Models
{
    [Display(Name = "Пользователь")]
    public class User : IdentityUser, IEntity
    {
        [Required]
        [Display(Name="Имя пользователя")]
        public override string UserName { get; set; }
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

        public string Display { get => UserName; }

        public object Key { get => Id; }
    }

    [Display(Name = "Роль")]
    public class Role : IdentityRole, IEntity
    {
        [Display(Name = "Пользователи")]
        public virtual HashSet<User> Users { get; set; } = new();

        private string _Display;
        [Display(Name = "Локализированное название")]
        public string Display { get => _Display; set => _Display = value; }

        public object Key { get => Id; }
    }

    [Display(Name ="Пользователь")]
    public class UserRef : Ref<User>
    {
        [Display(Name ="Роль")]
        public HashSet<Ref<Role>> Roles { get; set; }

        public UserRef(User User): base(User)
        {
            this.Roles = User.Roles.Select(e => new Ref<Role>(e)).ToHashSet();
        }
    }
}
