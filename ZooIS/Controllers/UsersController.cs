using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Controllers
{
    [Authorize(Policy ="AccessToUsers")]
    [Display(Name ="Пользователи")]
    public class UsersController : Controller
    {
        private readonly ZooISContext _context;

        public UsersController(ZooISContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Get(string? q, string? role, int page = 1)
        {
            return await _context.Users.AsQueryable()
                .Where(e => q == null || e.Display.ToLower().Contains(q))
                .Include(e => e.Roles)
                .Where(e => role == null || e.Roles.Select(e => e.Id).Contains(role))
                .OrderBy(e => e.Display)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? role)
        {
            return View(await Get(q, role));
        }
    }
}
