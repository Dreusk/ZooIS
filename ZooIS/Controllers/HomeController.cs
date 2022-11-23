using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZooISContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ZooISContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Display(Name ="Главная")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View("IndexNonAuthorized");
            string UserId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
            List<Log> Logs = (await _context.Logs
                .AsQueryable()
                .Where(e => e.UserId == UserId && e.PageTitle != "Главная")
                .OrderByDescending(e => e.ts)
                .Take(5)

                .ToListAsync())
                .GroupBy(e => e.Url)
                .Select(e => e.OrderByDescending(e => e.ts).First())
                .ToList();
            return View("IndexAuthorized", Logs);
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Display(Name ="Ошибка")]
        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
