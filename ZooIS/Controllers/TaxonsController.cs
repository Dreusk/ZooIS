using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
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
        public async Task<IActionResult> Index()
        {
            var zooISContext = _context.Taxons.Include(t => t.Parent);
            return View(await zooISContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Show(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxon = await _context.Taxons
                .Include(t => t.Parent)
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (taxon == null)
            {
                return NotFound();
            }

            return View(taxon);
        }

        [HttpGet]
        public IActionResult Crud()
        {
            ViewData["ParentGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crud([Bind("ScientificName,VernacularName,ParentGuid,Guid")] Taxon taxon)
        {
            if (ModelState.IsValid)
            {
                taxon.Guid = Guid.NewGuid();
                _context.Add(taxon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName", taxon.ParentGuid);
            return View(taxon);
        }

        // GET: Taxons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxon = await _context.Taxons.FindAsync(id);
            if (taxon == null)
            {
                return NotFound();
            }
            ViewData["ParentGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName", taxon.ParentGuid);
            return View(taxon);
        }

        // POST: Taxons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ScientificName,VernacularName,ParentGuid,Guid")] Taxon taxon)
        {
            if (id != taxon.Guid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taxon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxonExists(taxon.Guid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName", taxon.ParentGuid);
            return View(taxon);
        }

        // GET: Taxons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxon = await _context.Taxons
                .Include(t => t.Parent)
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (taxon == null)
            {
                return NotFound();
            }

            return View(taxon);
        }

        // POST: Taxons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var taxon = await _context.Taxons.FindAsync(id);
            _context.Taxons.Remove(taxon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaxonExists(Guid id)
        {
            return _context.Taxons.Any(e => e.Guid == id);
        }

        public class Hierarchy : Ref<Taxon>
        {
            [Display(Name = "Предметы")]
            public List<Hierarchy> Items { get; set; }
            [Display(Name = "Искомый?")]
            public bool isSearched { get; set; } = false;

            public Hierarchy(Taxon Taxon, string? q = null) : base(Taxon)
            {
                isSearched = q != null && Taxon.VernacularName.ToLower().Contains(q);
                Items = Taxon.Taxons
                    .OrderBy(e => e.VernacularName)
                    .Select(e => new Hierarchy(e, q))
                    .ToList();
            }
        }

        [HttpGet]
        [Display(Name = "Иерархия")]
        [Ignore]
        public async Task<List<Hierarchy>> get_hierarchy(string? q, TaxonRank level = TaxonRank.Species)
        {
            q = q?.ToLower();
            var SearchedItems = await _context.Taxons.AsQueryable()
                .Where(e => e.Rank <= level && (q == null || e.VernacularName.ToLower().Contains(q)))
                .ToListAsync();
            HashSet<Taxon> HighestParents = new();
            foreach (var Taxon in SearchedItems)
                HighestParents.Add(await Taxon.GetParentAt(_context, level));
            return (await _context.Taxons.AsQueryable()
                .Where(e => HighestParents.Select(e => e.Guid).Contains(e.Guid))
                .Include(e => e.Taxons)
                .ToListAsync())
                .Select(e => new Hierarchy(e, q))
                .ToList();
        }
    }
}
