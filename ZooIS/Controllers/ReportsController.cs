using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ZooIS.Models;
using ZooIS.Data;
using ZooIS.Components;

namespace ZooIS.Controllers
{
    [Authorize]
	[Display(Name="Отчеты")]
    public class ReportsController : Controller
    {
        public enum Report {
            [Display(Name="Построение генеологического древа")]
            Geneology,
            [Display(Name="Отчет о больных животных")]
            Sickness
        }

		private readonly IEnumerable<Concept<Report>> Reports = Enum.GetValues<Report>()
			.Select(e => (Concept<Report>)e);
															
		private readonly ZooISContext _context;
															
		private HierarchyCombobox.Params ReportInput;
		
		public ReportsController(ZooISContext context)
		{
			_context = context;
            ReportInput = new()
            {
                Flags = new()
                {
                    isSearchable = true,
                    isInstant = true,
                },
                Title = "Выберите отчет",
                Name = "Report",
                Placeholder = "Выберите отчет",
                Value = new(),
                Url = "Reports/get",
                SearchParams = new() { }
            };
        }
		
		private async Task GeneologyHandler(GeneologyParams Params) {
			Animal Animal = null;
		}
		
		private async Task SicknessHandler(SicknessParams Params) {
			
		}

		[HttpGet]
        public IEnumerable<Concept<Report>> get(string? q) {
            q = q.ToLower();
            return Reports
                .Where(e => q == null || e.Display.ToLower().Contains(q))
                .OrderBy(e => e.Display);
        }

		[HttpGet]
        public IActionResult Index() {
            return View();
        }
		
		[HttpGet]
		[Display(Name="Построение генеологического древа")]
		public IActionResult Geneology() {
			return View();
		}
		
		[HttpPost]
		[Display(Name="Построение генеологического древа")]
		public IActionResult Geneology(GeneologyParams Model) {
			GeneologyHandler(Model).ConfigureAwait(false);
			return View(Model);
		}
		
		[HttpGet]
		[Display(Name="Отчет о больных животных")]
		public IActionResult Sickness() {
			return View();
		}
		
		[HttpPost]
		[Display(Name="Отчет о больных животных")]
		public IActionResult Sickness(SicknessParams Model) {
			SicknessHandler(Model).ConfigureAwait(false);
			return View(Model);
		}
    }
}