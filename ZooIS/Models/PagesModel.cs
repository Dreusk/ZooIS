using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZooIS.Data;

namespace ZooIS.Models
{
    public class Page : IEntity
    {
        [Key]
        [Display(Name ="Домен")]
        public string Controller { get; set; }
        /// <summary>
        /// A localized name
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// A list of roles that can see this page in layout.
        /// </summary>
        [Display(Name = "Видимость")]
        public HashSet<string> VisibleFor { get; set; } = new();

        public object Key { get => Controller; }

        /// <summary>
        /// List of all pages in layout.
        /// </summary>
        public static readonly List<Page> AllPages = new(){
        new(){
            Controller = "Animals",
            Display = "Животные",
            VisibleFor = new HashSet<string>{"Caretaker"}
        },
        new(){
            Controller = "Taxons",
            Display = "Виды",
            VisibleFor = new HashSet<string>{"Caretaker"}
        },
        new(){
            Controller = "Users",
            Display = "Пользователи",
            VisibleFor = new HashSet<string>{"Admin"}
        },
        new(){
            Controller = "Reports",
            Display = "Отчеты",
            VisibleFor = new HashSet<string>{"Caretaker"}
        }};
    }
}
