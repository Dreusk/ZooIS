using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ZooIS.Data;
using ZooIS.Models;
using System;
using System.Diagnostics.Contracts;

namespace ZooIS.Components
{
    public class HierarchyCombobox: ViewComponent
    {
        public class Params
        {
            [Display(Name ="Флаги поведения")]
            public class BehaviorFlags
            {
                [Display(Name ="Можно искать")]
                public bool isSearchable { get; set; } = false;
                [Display(Name ="Выбор нескольких вариантов")]
                public bool isMultiselectable { get; set; } = false;
                [Display(Name = "Обязательное поле")]
                public bool isRequired { get; set; } = false;
                [Display(Name = "Мгновенная загрузка")]
                public bool isInstant { get; set; } = false;
            }
            [Display(Name="Имя")]
            public string Name { get; set; }
            [Display(Name = "Титул")]
            public string Title { get; set; } = "";
            [Display(Name = "Значения")]
            public List<Ref<IEntity>> Value { get; set; } = new();
			[Display(Name = "Предметы")]
			public List<Ref<IEntity>> Items { get; set; } = new();
			[Display(Name = "Максимальное число предметов")]
            public uint? MaxCount { get; set; } = null;
            [Display(Name ="Плейсхолдер")]
            public string Placeholder { get; set; } = "Выберите...";
            [Display(Name = "Выключенный плейсхолдер")]
            public string DisabledPlaceholder { get; set; } = "Выберите...";
            [Display(Name = "Урл")]
            public string? Url { get; set; } = null;
            public BehaviorFlags Flags { get; set; }
            [Display(Name = "Параметры поиска")]
            public Dictionary<string, object> SearchParams { get; set; } = new();
            /// <summary>
            /// A list of inputs id`s;
            /// </summary>
            [Display(Name ="Зависимости")]
            public HashSet<string> Dependencies { get; set; } = new();
        }

        public IViewComponentResult Invoke(Params Model)
        {
            return View("default", Model);
        }
    }
}