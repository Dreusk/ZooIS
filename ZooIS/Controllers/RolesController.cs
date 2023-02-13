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
        public async Task<List<Role>> get(string? q)
        {
            q = q?.ToLower();
            return await _context.Roles.AsQueryable()
                .Where(e => q == null || e.Name.ToLower().Contains(q))
                .OrderBy(e => e.Name)
                .ToListAsync();
        }
    }
}
