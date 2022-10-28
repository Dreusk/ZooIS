using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ZooIS.Models
{
    public enum ActionType { Enter, Quit }

    [Display(Name ="Лог")]
    public class Log : Entity
    {
        [Display(Name ="Временная метка")]
        public DateTime ts { get; set; }
        [Display(Name = "URL")]
        public string Url { get; set; }
        [Display(Name = "Титул страницы")]
        public string PageTitle { get; set; }
        [Display(Name = "Действие")]
        public ActionType Action { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name = "Пользователь")]
        public virtual IdentityUser User { get; set; }

        public override string Display { get => PageTitle; }
    }
}