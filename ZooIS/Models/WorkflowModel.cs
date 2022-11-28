using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using ZooIS.Data;

namespace ZooIS.Models
{
    public enum AlertLevel {
        [Display(Name = "Нормальный")]
        Regular,
        [Display(Name = "Внимание")]
        Warning,
        [Display(Name = "Тревога")]
        Alert }

    public enum AlertType {
        [Display(Name = "Информация")]
        Info,
        [Display(Name = "Успех")]
        Success,
        [Display(Name ="Неудача")]
        Fail }

    [Display(Name = "Оповещение")]
    public class Alert : Entity
    {
        [Required]
        [Display(Name = "Краткое сообщение")]
        [MaxLength(255)]
        public string Message { get; set; }
        [Display(Name = "Ссылка до отображениея")]
        public string Url { get; set; }
        [Required]
        [Display(Name = "Уровень")]
        public virtual AlertLevel Level { get; set; } = AlertLevel.Regular;
        [Required]
        [Display(Name = "Тип")]
        public virtual AlertType Type { get; set; } = AlertType.Info;
        
        [Display(Name = "Прочитано?")]
        public bool isRead { get; set; } = false;

        [Display(Name = "Временная метка")]
        [Required]
        public DateTime ts { get; set; } = DateTime.Now;

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name ="Пользователь")]
        public User? User { get; set; }

        public Alert(): base() { }

        public override string Display { get => Message; }
    }
}
