using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;
using ZooIS.Data;
using Microsoft.EntityFrameworkCore;

namespace ZooIS.Models
{
	[Owned]
    [Display(Name = "Имя")]
    public class Name: IEntity
	{
		[Required]
		[MaxLength(60)]
		[Display(Name="Имя")]
		public string GivenName { get; set; }
		[Required]
		[MaxLength(60)]
		[Display(Name ="Фамилия")]
		public string FamilyName { get; set; }
		[MaxLength(100)]
		[Display(Name ="Доп. имя")]
		public string? ThirdName { get; set; }
		[Key]
		public Guid EmployeeGuid { get; set; }
		[Display(Name="Сотрудник")]
		public virtual Employee Employee { get; set; }

		public string Display { get => $"{FamilyName} {GivenName}"; }

		public object Key { get => EmployeeGuid; }
	}

    [Display(Name= "Сотрудник")]
	public class Employee: Entity {
        [Required]
		[Display(Name="Имя")]
		public Name Name{ get; set; }
		[Display(Name = "Дата приёма")]
		public DateTime EnrollDate { get; set; } = DateTime.Now;
		
		public string UserId  { get; set; }
		[Display(Name="Пользователь")]
		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		public override string Display { get => Name.Display; }

		public User AsUser() => User;
	}
}
