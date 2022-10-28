using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
        [Required]
        [Display(Name = "Уровень")]
        public AlertLevel Level { get; set; }
        [Required]
        [Display(Name = "Тип")]
        public AlertType Type { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        [Display(Name ="Пользователь")]
        public IdentityUser? User { get; set; }

        public Alert(): base() { }

        public override string Display { get => Message; }
    }
}
