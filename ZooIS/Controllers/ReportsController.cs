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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

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
                Url = "/Reports/get",
                SearchParams = new() { }
            };
        }
		
		//Handlers
		private async Task<List<GeneologyResult>> GeneologyHandler(GeneologyParams Params) {
			List<GeneologyResult> Result = new();
			if (Params?.Animal is null)
				return Result;
			using (SqlConnection Connection = new(_context.Database.GetConnectionString()))
			{
				Connection.Open();
				using (SqlCommand Procedure = new($"exec GetGeneology '{Params.Animal}'", Connection))
				{
					using (SqlDataReader Query = await Procedure.ExecuteReaderAsync(CommandBehavior.CloseConnection))
					{
						while (await Query.ReadAsync())
							Result.Add(new(Query));
					}
				}
			}
			Result = Result
				.Join(_context.Animals, rel => rel.ChildrenGuid, animal => animal.Guid, (rel, animal) =>
				{
					rel.Children = animal;
					return rel;
				})
                .Join(_context.Animals, rel => rel.ParentsGuid, animal => animal.Guid, (rel, animal) =>
                {
                    rel.Parent = animal;
                    return rel;
                })
                .ToList();
			return Result;
		}
		
		private async Task<List<SicknessResult>> SicknessHandler(SicknessParams Params) {
			return new List<SicknessResult>();
		}

		[Ignore]
		[HttpGet]
        public IEnumerable<Concept<Report>> get(string? q) {
            q =  q?.ToLower();
            return Reports
                .Where(e => q == null || e.Display.ToLower().Contains(q))
                .OrderBy(e => e.Display);
        }

		[Ignore]
		[HttpGet]
		public IActionResult RedirectToReport(Report? report)
		{
			switch(report)
			{
				case Report.Sickness:
					return RedirectToAction("Sickness");
				case Report.Geneology:
					return RedirectToAction("Geneology");
                default:
					return Redirect("Index");
			}
		}

		[HttpGet]
        public IActionResult Index(Report? report) {
			if (report is not null)
				return RedirectToReport(report);
			ViewBag.ReportInput = ReportInput;
			return View();
        }
		
		[HttpGet]
		[Display(Name="Построение генеологического древа")]
		public IActionResult Geneology(Report? report) {
			ReportInput.Value = new() { Report.Geneology.GetRef() };
			ViewBag.ReportInput = ReportInput;
			return View();
		}
		
		[HttpPost]
		[Display(Name="Построение генеологического древа")]
		public async Task<IActionResult> Geneology(GeneologyParams Model) {
			return View("Show/Geneology", await GeneologyHandler(Model));
		}
		
		[HttpGet]
		[Display(Name="Отчет о больных животных")]
		public IActionResult Sickness(Report? report) {
			ReportInput.Value = new() { Report.Sickness.GetRef() };
			ViewBag.ReportInput = ReportInput;
			return View();
		}
		
		[HttpPost]
		[Display(Name="Отчет о больных животных")]
		public async Task<IActionResult> Sickness(SicknessParams Model) {
			return View("Show/Sickness", await SicknessHandler(Model));
		}
    }
}