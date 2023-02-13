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
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<List<User>> get(string? q, string? role, int page = 1)
        {
            q = q?.ToLower();
            return await _context.Users.AsQueryable()
                .Where(e => q == null || e.UserName.ToLower().Contains(q))
                .Include(e => e.Roles)
                .Where(e => role == null || e.Roles.Select(e => e.Id).Contains(role))
                .OrderBy(e => e.UserName)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? role, int page)
        {
            List<User> Users = await get(q, role);
            Role? Role = role is null ? null :
                await _context.Roles.FindAsync(role);
            ViewData["Page"] = page;
            ViewData["Params"] = new Dictionary<string, IEntity>
            {
                { "Роль", Role }
            };
            return View(Users);
        }

        [HttpGet]
        public async Task<IActionResult> Show(Guid id)
        {
            return View(await _context.Users.FindAsync(id));
        }
    }
}
