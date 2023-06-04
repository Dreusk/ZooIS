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
using System.Xml.Linq;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml.DataValidation;

namespace ZooIS.Controllers
{
	[Authorize]
	[Display(Name = "Отчеты")]
	public class ReportsController : Controller
	{
		private readonly IEnumerable<Ref<IEntity>> Reports = Enum.GetValues<ReportType>()
			.Select(e => e.GetRef());

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
				Url = "/Reports/getTypes",
				SearchParams = new() { }
			};
		}

		//Handlers
		[Ignore]
		private async Task<Guid?> GeneologyHandler(Guid? Animal) {
			List<GeneologyResult> Result = new();
			if (Animal is null)
				return null;
			GeneologyParams Params = new() { Animal = new(await _context.Animals.FindAsync(Animal)) };
			using (SqlConnection Connection = new(_context.Database.GetConnectionString()))
			{
				Connection.Open();
				using (SqlCommand Procedure = new($"exec GetGeneology '{Params.Animal.Id}'", Connection))
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
			Report Report = new()
			{
				Type = ReportType.Geneology,
				RequesterId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value,
				XmlParams = Params.ToXml(),
				XmlResult = Result.ToXml()
			};
			_context.Reports.Add(Report);
			await _context.SaveChangesAsync();
			return Report.Guid;
		}

		[Ignore]
		private async Task<Guid?> SicknessHandler(SicknessParams Params) {
			List<SicknessResult> Result = new();
			Report Report = new()
			{
				Type = ReportType.Sickness,
				RequesterId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value,
				XmlParams = Params.ToXml(),
				XmlResult = Result.ToXml()
			};
			_context.Reports.Add(Report);
			await _context.SaveChangesAsync();
			return Report.Guid;
		}

		[Ignore]
		[HttpGet]
		public List<Ref<IEntity>> getTypes(string? q) {
			q = q?.ToLower();
			return Reports
				.Where(e => q == null || e.Display.ToLower().Contains(q))
				.OrderBy(e => e.Display)
				.ToList();
		}

		[Ignore]
		[HttpGet]
		public async Task<List<Ref<Report>>> get(ReportType? type = null, uint count = 20, uint page = 1)
		{
			string UserId = User.Claims.First(e => e.Type == ClaimTypes.NameIdentifier).Value;
            return (await _context.Reports.AsQueryable()
				.Where(e => e.RequesterId == UserId &&
				(type == null || e.Type == type))
				.OrderByDescending(e => e.ts)
				.Skip((int)((page-1)*count))
				.Take((int)count)
				.ToListAsync())
				.Select(e => new Ref<Report>(e))
				.ToList();
        }

		[HttpGet]
        public IActionResult Index(ReportType? report) {
            if (report is not null && report != ReportType.Ready)
				return RedirectToAction(Enum.GetName((ReportType)report));
            ReportInput.Value = new() { ReportType.Ready.GetRef() };
            ViewBag.ReportInput = ReportInput;
			return View();
        }
		
		[HttpGet]
		[Display(Name="Построение генеологического древа")]
		public IActionResult Geneology() {
			ReportInput.Value = new() { ReportType.Geneology.GetRef() };
			ViewBag.ReportInput = ReportInput;
			return View();
		}
		
		[HttpPost]
		[Display(Name="Построение генеологического древа")]
		public async Task<IActionResult> Geneology(Guid? Animal) {
            return Redirect($"Show/{await GeneologyHandler(Animal)}");
		}
		
		[HttpGet]
		[Display(Name="Отчет о больных животных")]
		public IActionResult Sickness() {
			ReportInput.Value = new() { ReportType.Sickness.GetRef() };
			ViewBag.ReportInput = ReportInput;
			return View();
		}
		
		[HttpPost]
		[Display(Name="Отчет о больных животных")]
		public async Task<IActionResult> Sickness(SicknessParams Model) {
			return Redirect($"Show/{await SicknessHandler(Model)}");
		}

		[HttpGet]
		[Route("[controller]/[action]/{id}")]
		public async Task<IActionResult> Show(Guid? id)
		{
            if (id == null)
                return NotFound();
			Report? Report = await _context.Reports.FindAsync(id);
			if (Report == null)
				return NotFound();
			return View($"Show/{Enum.GetName(Report.Type)}", Report);
        }

		[Ignore]
		[HttpGet]
		[Route("[controller]/{id}/Export")]
		public async Task<FileContentResult> Export(Guid? Id)
		{
			if (Id is null)
				return null;
			Report? Report = await _context.Reports.FindAsync(Id);
			if (Report is null)
				return null;
            using (ExcelPackage excel = new())
            {
                ExcelWorksheet report = excel.Workbook.Worksheets.Add(Report.Type.GetDisplay());
				switch (Report.Type)
				{
					case ReportType.Geneology:
						new GeneologyResult().InsertHeaders(ref report);
						foreach (GeneologyResult row in Report.XmlResult.Deparse<List<GeneologyResult>>())
							row.InsertRow(ref report);
						break;
					case ReportType.Sickness:
                        new SicknessResult().InsertHeaders(ref report);
						foreach (SicknessResult row in Report.XmlResult.Deparse<List<SicknessResult>>())
							row.InsertRow(ref report);
						break;
				}
				return File(excel.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Report.ts.ToString("yyyyMMdd_hhmm")} {Report.Type.GetDisplay()}");
            }
        }
    }
}