using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Components
{
    public class AlertComponent: ViewComponent
    {
		private readonly ZooISContext _context;
		
		public AlertComponent(ZooISContext context)  => _context = context;
		
        public async Task<IViewComponentResult> InvokeAsync()
        {
			string UserId = HttpContext.User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
			Alert? Alert = await _context.Alerts.AsQueryable()
								.Where(e => !e.isRead && e.UserId == UserId)
								.OrderByDescending(e => e.Level)
								.FirstOrDefaultAsync();
			return View("Default", Alert?.Level);
        }
    }
}
