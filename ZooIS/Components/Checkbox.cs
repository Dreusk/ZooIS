using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;
using ZooIS.Data;

namespace ZooIS.Components
{
    public class Checkbox: ViewComponent
    {
        public class Params
        {
            public string Name;
            public string? Label;
            public bool isChecked = false;

            public Params(string Name) => this.Name = Name;
            public Params(string Name, bool isChecked)
            {
                this.Name = Name;
                this.isChecked = isChecked;
            }
        }

        public IViewComponentResult Invoke(Params Params)
        {
            return View("default", Params);
        }
    }
}
