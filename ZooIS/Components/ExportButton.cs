using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZooIS.Components
{
    public class ExportButton: ViewComponent
    {
        public class Params
        {
            public class BehaviorFlags
            {
                public bool isDisabled { get; set; } = false;
            }

            [Display(Name ="Адрес")]
            public string Url { get; set; }
            public object Id { get; set; }

            public BehaviorFlags Flags { get; set; } = new();

            public Params() { }
        }

        public IViewComponentResult Invoke(Params Model)
        {
            return View("default", Model);
        }
    }
}
