using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using ZooIS.Data;

namespace ZooIS.Models
{
    public enum ActionType { Enter, Quit }

    [Display(Name ="Лог")]
    public class Log : Entity
    {
        [Display(Name = "Временная метка")]
        public DateTime ts { get; set; } = DateTime.Now;
        [Display(Name = "URL")]
        [MaxLength(2048)]
        public string Url { get; set; }
        [Display(Name = "Титул страницы")]
        [MaxLength(64)]
        public string PageTitle { get; set; }
        
        public object ActionId { get; set; }
        [Display(Name = "Действие")]
        public virtual Concept<ActionType> Action { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name = "Пользователь")]
        public virtual User User { get; set; }

        public override string Display { get => $"{ts.Day}, {DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(ts.DayOfWeek)} - {ts.ToString("hh:mm")} - {PageTitle}"; }  
    }
}