using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ZooIS.Models;
using ZooIS.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ZooIS.Controllers
{
    [Authorize(Policy = "AccessToRoles")]
    [Display(Name ="Роли")]
    public class RolesController : Controller
    {
        private readonly ZooISContext _context;

        public RolesController(ZooISContext context) => _context = context;


        [HttpGet]
        [Display(Name = "Текущий пользователь")]
        [Ignore]
        public async Task<ActionResult<List<Ref<Role>>>> by_current_user()
        {
            string UserId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
            return await _context.UserRoles.AsQueryable()
                .Where(e => e.UserId == UserId)
                .Join(_context.Roles, e => e.RoleId, e => e.Id, (e, role) => role)
                .OrderBy(e => e.Display)
                .Select(e => new Ref<Role>(e))
                .ToListAsync();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Role>> get(string? q, int page = 1, string? id = null)
        {
            const int count = 20;
            if (id is not null)
                return new() { await _context.Roles.FindAsync(id) };
            q = q?.ToLower();
            return await _context.Roles.AsQueryable()
                .Where(e => q == null || (e.Name + e.Display).ToLower().Contains(q))
                .OrderBy(e => e.Display)
                .Skip((page - 1) * count)
                .Take(count)
                .ToListAsync();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Ref<Role>>> getRefs(string? q, int page = 1, string? id = null)
        {
            return (await get(q, page, id))
                .Select(e => new Ref<Role>(e))
                .ToList();
        }

        [HttpGet]
        public IActionResult Index(string? q, int page = 1)
        {
            ViewData["Page"] = page;
            ViewData["Params"] = new Dictionary<string, IEntity>
            {
            };
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Show(string? id)
        {
            if (id == null)
                return NotFound();
            Role role = await _context.Roles
                .FirstAsync(e => e.Id == id);
            if (role is null)
                return NotFound();
            ViewBag.AvailablePages = Page.PagesForRole(role.Name);
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> Crud(string? id)
        {
            Role? role = id != null ? await _context.Roles
                .FirstOrDefaultAsync(e => e.Id == id)
                : null;
            if (id is not null && role is null)
                return NotFound();
            return View(role);
        }
    }
}
