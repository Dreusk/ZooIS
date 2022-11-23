using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Components
{
    public class UserComponent: ViewComponent
    {
        private readonly ZooISContext _context;

        public UserComponent(ZooISContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            User? User = await _context.Users.AsQueryable()
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
            return View("Default", User);
        }
    }
}