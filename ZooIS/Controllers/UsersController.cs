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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;

namespace ZooIS.Controllers
{
    [Authorize(Policy ="AccessToUsers")]
    [Display(Name ="Пользователи")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ZooISContext _context;

        public UsersController(UserManager<User> userManager, ZooISContext context)
        {
            this.userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Ignore]
        public async Task<List<User>> get(string? q, string? role, int page = 1, string? id = null)
        {
            const int count = 20;
            if (id is not null)
                return new() { await _context.Users.FindAsync(id) };
            q = q?.ToLower();
            return await _context.Users.AsQueryable()
                .Include(e => e.Employee)
                .Where(e => q == null || (e.UserName + e.Employee.Name.Display).ToLower().Contains(q))
                .Include(e => e.Roles)
                .Where(e => role == null || e.Roles.Select(e => e.Id).Contains(role))
                .OrderBy(e => e.UserName)
                .Skip((page - 1) * count)
                .Take(count)
                .ToListAsync();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<UserRef>> getRefs(string? q, string? role, int page = 1, string? id = null)
        {
            return (await get(q, role, page, id))
                .Select(e => new UserRef(e))
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? role, int page)
        {
            Role? Role = role is null ? null :
                await _context.Roles.FindAsync(role);
            ViewData["Page"] = page;
            ViewData["Params"] = new Dictionary<string, IEntity>
            {
                { "Роль", Role }
            };
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Show(string? id)
        {
            if (id == null)
                return NotFound();
            User user = await _context.Users
                .Include(e => e.Employee)
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (user is null)
                return NotFound();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Crud(string? id)
        {
            User? user = id != null ? await _context.Users
                .Include(e => e.Employee)
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.Id == id)
                : null;
            if (id is not null && user is null)
                return NotFound();
            ViewBag.Roles = user is not null ? user.Roles.Select(e => new Ref<IEntity>(e)).ToList() : null;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Crud()
        {
            User user = new();
            var Form = Request.Form;
            if (Form["Id"] != "")
                user = await _context.Users
                    .Include(e => e.Employee)
                    .Include(e => e.Roles)
                    .FirstOrDefaultAsync(e => e.Id == Form["Id"].ToString());
            user.UserName = Form["UserName"];
            if (user.Employee is null && Form["Employee"].ToString().Count() != 0)
            {
                Employee employee = new()
                {
                    Name = new(Form["Employee"])
                };
                user.Employee = employee;
            }
            foreach (string RoleId in Form["Roles"])
                user.Roles.Add(await _context.Roles.FindAsync(RoleId));
            TryValidateModel(user);
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            await _context.SaveChangesAsync();
            await userManager.UpdateAsync(user);
            return RedirectToAction("Show", "Users", new { id = user.Id });
        }
    }
}
