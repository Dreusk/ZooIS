using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Linq;
using ZooIS.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Operators;

namespace ZooIS.Models
{
	[Owned]
    [Display(Name = "Имя")]
    public class Name: IEntity
	{
		[Required]
		[MaxLength(60)]
		[Display(Name = "Имя")]
		public string GivenName { get; set; } = "";
		[Required]
		[MaxLength(60)]
		[Display(Name = "Фамилия")]
		public string FamilyName { get; set; } = "";
		[MaxLength(100)]
		[Display(Name ="Доп. имя")]
		public string? ThirdName { get; set; }
		[Key]
		public Guid EmployeeGuid { get; set; }
		[Display(Name="Сотрудник")]
		public virtual Employee Employee { get; set; }

		public Name() : base() {}
		public Name(string From)
		{
			if (From.Count() == 0)
				return;
			var Values = From.Split(" ");
			switch(Values.Count())
			{
				case 1:
					GivenName = Values[0];
					break;
				case 2:
					FamilyName = Values[0];
					GivenName = Values[1];
					break;
				case 3:
                    FamilyName = Values[0];
                    GivenName = Values[1];
					ThirdName = Values[2];
					break;
                default:
                    FamilyName = Values[0];
                    GivenName = Values[1];
                    ThirdName = String.Join(' ', Values.Skip(2));
					break;
            }
			
		}

		[Display(Name="ФИО")]
		public string Display { get => $"{FamilyName} {GivenName}"; }

		public object Key { get => EmployeeGuid; }

		public static bool operator== (Name left, Name right)
		{
			return left.GivenName == right.GivenName &&
				left.FamilyName == right.FamilyName &&
				left.ThirdName == right.ThirdName;
		}

		public static bool operator!= (Name left, Name right)
		{
			return !(left == right);
		}

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Display(Name= "Сотрудник")]
	public class Employee: Entity {
        [Required]
		[Display(Name="ФИО")]
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
