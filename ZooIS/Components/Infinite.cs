using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZooIS.Components
{
    public class Infinite: ViewComponent
    {
        public class Params
        {
            [Required]
            [Display(Name = "Урл")]
            public string Url { get; set; }
        }

        public IViewComponentResult Invoke(Params Model)
        {
            return View("Default", Model);
        }
    }
}
