using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZooIS.Data;
using ZooIS.Models;

namespace ZooIS.Controllers
{
    [Display(Name = "Животные")]
    public class AnimalsController : Controller
    {
        private readonly ZooISContext _context;

        public AnimalsController(ZooISContext context)
        {
            _context = context;
        }

        // GET: Animals
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Animal.Include(a => a.Species);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Animals/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                .Include(a => a.Species)
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["SpeciesGuid"] = new SelectList(_context.Species, "Guid", "ScientificName");
            return View();
        }

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nickname,Age,BirthDate,SpeciesGuid,Sex,Guid")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                animal.Guid = Guid.NewGuid();
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpeciesGuid"] = new SelectList(_context.Species, "Guid", "ScientificName", animal.SpeciesGuid);
            return View(animal);
        }

        // GET: Animals/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            ViewData["SpeciesGuid"] = new SelectList(_context.Species, "Guid", "ScientificName", animal.SpeciesGuid);
            return View(animal);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Nickname,Age,BirthDate,SpeciesGuid,Sex,Guid")] Animal animal)
        {
            if (id != animal.Guid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Guid))
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
            ViewData["SpeciesGuid"] = new SelectList(_context.Species, "Guid", "ScientificName", animal.SpeciesGuid);
            return View(animal);
        }

        // GET: Animals/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                .Include(a => a.Species)
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var animal = await _context.Animal.FindAsync(id);
            _context.Animal.Remove(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(Guid id)
        {
            return _context.Animal.Any(e => e.Guid == id);
        }
    }
}
