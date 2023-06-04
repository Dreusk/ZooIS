using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Controllers
{
    [Display(Name = "Таксоны")]
    public class TaxonsController : Controller
    {
        private readonly ZooISContext _context;

        public TaxonsController(ZooISContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Taxon>> get(string? q, int page = 1, TaxonRank Rank = TaxonRank.Species, Guid? taxon = null, string? id = null)
        {
            const int count = 20;
            if (id is not null)
                return new() { await _context.Taxons.FindAsync(id) };
            q = q?.ToLower();
            return await _context.Taxons.AsQueryable()
                .Where(e => e.Rank == Rank && q == null || (e.ScientificName + e.VernacularName).ToLower().Contains(q))
                .OrderBy(e => e.VernacularName)
                .Skip((page - 1) * count)
                .Take(count)
                .ToListAsync();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<TaxonRef>> getRefs(string? q, int page = 1, TaxonRank Rank = TaxonRank.Species, Guid? taxon = null, string? id = null)
        {
            return (await get(q, page, Rank, taxon, id))
                .Select(e => new TaxonRef(e))
                .ToList();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<TaxonRef>> get_offByOne(string? q, int page = 1, TaxonRank Rank = TaxonRank.Species, Guid? taxon = null, string? id = null)
        {
            return await getRefs(q, page, Rank + 1, taxon, id);
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Taxon>> getSpecies(string? q, int page = 1, Guid? taxon = null, string? id = null)
        {
            const int count = 20;
            if (id is not null)
                return new() { await _context.Taxons.FindAsync(id) };
            Taxon? rootTaxon = taxon is null ? null :
                await _context.Taxons.FindAsync(taxon);
            q = q?.ToLower();
            if (rootTaxon is null)
            {
                return await _context.Taxons.AsQueryable()
                    .Where(e => e.Rank == TaxonRank.Species && q == null || (e.ScientificName + e.VernacularName).ToLower().Contains(q))
                    .OrderBy(e => e.VernacularName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync();
            }
            else
            {
                return (await rootTaxon.GetSpecies(_context))
                    .Where(e => q == null || (e.ScientificName + e.VernacularName).ToLower().Contains(q))
                    .OrderBy(e => e.VernacularName)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToList();
            }
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Ref<Taxon>>> getSpeciesRefs(string? q, int page = 1, Guid? taxon = null, string? id = null)
        {
            return (await getSpecies(q, page, taxon, id))
                .Select(e => new Ref<Taxon>(e))
                .ToList();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<TaxonRef>> getHierarchy(string? q, TaxonRank level = TaxonRank.Species, int? depth = null)
        {
            q = q?.ToLower();
            var SearchedItems = await _context.Taxons.AsQueryable()
                .Where(e => e.Rank <= level &&
                    depth == null ? true : e.Rank >= level + depth &&
                    (q == null || e.VernacularName.ToLower().Contains(q)))
                .ToListAsync();
            HashSet<Taxon> HighestParents = new();
            foreach (var Taxon in SearchedItems)
                HighestParents.Add(await Taxon.GetParentAt(_context, level));
            return (await _context.Taxons.AsQueryable()
                .Where(e => HighestParents.Select(e => e.Guid).Contains(e.Guid))
                .Include(e => e.Taxons)
                .ToListAsync())
                .Select(e => new TaxonRef(e, q))
                .ToList();
        }

        [HttpGet]
        [Ignore]
        public async Task<List<Taxon>?> taxon_getHierarchy(Guid? rootTaxon)
        {
            List<Taxon> Result = new();
            if (rootTaxon is null)
                return null;
            using (SqlConnection Connection = new(_context.Database.GetConnectionString()))
            {
                Connection.Open();
                using (SqlCommand Procedure = new($"exec taxon_GetHierarchy '{rootTaxon}'", Connection))
                {
                    using (SqlDataReader Query = await Procedure.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await Query.ReadAsync())
                        {
                            Taxon tmp = new();
                            tmp.Guid = (Guid)Query["Guid"];
                            tmp.ScientificName = (string)Query["ScientificName"];
                            tmp.VernacularName = (string)Query["VernacularName"];
                            tmp.Rank = (TaxonRank)Query["Rank"];
                            Result.Add(tmp);
                        }
                    }
                }
            }
            return Result;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int page = 1, Guid? taxon = null)
        {
            Taxon? rootTaxon = taxon is null ? null :
                await _context.Taxons.FindAsync(taxon);
            ViewData["Page"] = page;
            ViewData["Params"] = new Dictionary<string, IEntity>
            {
                { "Таксон", rootTaxon }
            };
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Show(Guid? id)
        {
            if (id == null)
                return NotFound();
            Taxon taxon = await _context.Taxons
                .FirstOrDefaultAsync(e => e.Guid == id);
            if (taxon is null)
                return NotFound();
            ViewBag.Hierarchy = await taxon_getHierarchy(taxon.Guid);
            return View(taxon);
        }

        [HttpGet]
        public async Task<IActionResult> Crud(Guid? id)
        {
            Taxon? taxon = id != null ? await _context.Taxons
                .Include(e => e.Parent)
                .FirstOrDefaultAsync(e => e.Guid == id)
                : null;
            if (id is not null && taxon is null)
                return NotFound();
            ViewBag.ParentRef = taxon?.Parent is not null ? new Ref<IEntity>(taxon.Parent) : null;
            ViewBag.RankRef = taxon is not null ? taxon?.Rank.GetRef() : null;
            ViewBag.Ranks = Enum.GetValues<TaxonRank>().OrderByDescending(e => e).Select(E => E.GetRef()).ToList();
            return View(taxon);
        }

        [HttpPost]
        public async Task<IActionResult> Crud()
        {
            Taxon taxon = new();
            var Form = Request.Form;
            if (Form["Id"] != "")
                taxon = await _context.Taxons
                    .FirstOrDefaultAsync(e => e.Guid == new Guid(Form["Id"].ToString()));
            taxon.ScientificName = Form["ScientificName"];
            taxon.VernacularName = Form["VernacularName"];
            taxon.Rank = Enum.Parse<TaxonRank>(Form["Rank"].ToString());
            Guid? parentGuid = new(Form["Parent"]);
            taxon.Parent = parentGuid is not null ? await _context.Taxons.FindAsync(new Guid(Form["Parent"].ToString())) : null;
            TryValidateModel(taxon);
            if (!ModelState.IsValid)
            {
                return View(taxon);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Show", "Taxons", new { id = taxon.Guid });
        }
    }
}
