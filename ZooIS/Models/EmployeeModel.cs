using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;

namespace ZooIS.Models
{
    [Display(Name="Работник")]
	public class Employee: Entity {
        [Required]
        [MaxLength(100)]
		[Display(Name="Имя")]
		public string Name { get; set; }
		[Display(Name="Дата приёма")]
		public DateTime EnrollDate { get; set; }
		
		protected string UserId  { get; set; }
		[Display(Name="Пользователь")]
		[ForeignKey("UserId")]
		protected virtual User User { get; set; }
		
		public User AsUser() => User;
	}
}
