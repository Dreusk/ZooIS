using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ZooIS.Models;

namespace ZooIS.Components
{
    public class PagesLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<string> UserRoles = UserClaimsPrincipal.Claims.Where(e => e.Type == ClaimTypes.Role).Select(e => e.Value).ToList();
            List<Page> Pages = UserRoles.SelectMany(role => Page.PagesForRole(role)).ToList();
            return View("Default", Pages);
        }
    }
}
