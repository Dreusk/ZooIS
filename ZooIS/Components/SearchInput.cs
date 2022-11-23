using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;
using ZooIS.Data;

namespace ZooIS.Components
{
    public class SearchInput: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default", "");
        }
    }
}
