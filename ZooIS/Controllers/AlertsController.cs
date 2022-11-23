using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ZooIS.Models;
using ZooIS.Data;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ZooIS.Controllers
{
    [Display(Name ="Оповещения")]
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly ZooISContext _context;

        public class AlertResult: Entity
        {
            [Display(Name = "Сообщение")]
            public string Message { get; set; }
            [Display(Name = "Уровень")]
            public Concept<AlertLevel> Level { get; set; }
            [Display(Name = "Тип")]
            public Concept<AlertType> Type { get; set; }

            public AlertResult(Alert Alert)
            {
                this.Guid = Alert.Guid;
                this.Message = Alert.Message;
                this.Level = Alert.Level;
                this.Type = Alert.Type;
            }

            public override string Display { get => Message; }
        }

        public AlertsController(ZooISContext context) => _context = context;

        [HttpGet]
        [Display(Name ="Текущий пользователь")]
        [Ignore]
        public async Task<List<AlertResult>> by_current_user()
        {
            string UserId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
            return await _context.Alerts.AsQueryable()
                .Where(e => !e.isRead && e.UserId == UserId)
                .OrderByDescending(e => e.ts)
                .Take(7)
                .Select(e => new AlertResult(e))
                .ToListAsync();
        }

        [HttpPatch]
        [Route("/[controller]/{id}/[action]")]
        [Display(Name ="Отменить прочитанным")]
        [Ignore]
        public async Task<string> mark_as_read(Guid id)
        {
            Alert alert = await _context.Alerts.FindAsync(id);
            alert.isRead = true;
            await _context.SaveChangesAsync();
            return "ok";
        }

        [HttpGet]
        public async Task<ViewResult> Index()
        {
            string UserId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
            List<Alert> Alerts = await _context.Alerts.AsQueryable()
                .Where(e => e.UserId == UserId)
                .OrderByDescending(e => e.ts)
                .ToListAsync();
            ViewData["Title"] = "Оповещения";
            return View(Alerts);
        }
    }
}
