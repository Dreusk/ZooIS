using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;
using ZooIS.Data;

namespace ZooIS.Components
{
    public class IndexSearch: ViewComponent
    {
        public IViewComponentResult Invoke(string InnerForm)
        {
            return View("Default", InnerForm);
        }
    }
}
