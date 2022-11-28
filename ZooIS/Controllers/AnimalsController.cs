using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZooIS.Data;
using ZooIS.Data.Migrations;
using ZooIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ZooIS.Controllers
{
    [Display(Name = "Животные")]
    public class AnimalsController : Controller
    {
        private readonly ZooISContext _context;
        private readonly IWebHostEnvironment _enviroment;

        public AnimalsController(ZooISContext context, IWebHostEnvironment enviroment) {
            _context = context;
            _enviroment = enviroment;
        }

        [Ignore]
        [HttpGet]
        public async Task<List<Animal>> get(string? q, Guid? species, int page = 1, Guid? id = null)
        {
            if (id is not null)
                return new() { await _context.Animals.FindAsync(id) };
            q = q?.ToLower();
            Taxon? rootTaxon = species is null ? null :
                await _context.Taxons.FindAsync(species);
            HashSet<Taxon>? Taxons = species is null ? null :
                (await rootTaxon.GetSpecies(_context)).ToHashSet();
            return await _context.Animals.AsQueryable()
                          .Where(e => q == null || e.Name.ToLower().Contains(q))
                          .Include(e => e.Species)
                          .Where(e => species == null || Taxons.Select(e => e.Guid).Contains(e.SpeciesGuid))
                          .OrderBy(e => e.Name)
                          .Skip((page - 1) * 18)
                          .Take(18)
                          .ToListAsync();
        }

        [Ignore]
        [HttpGet]
        public async Task<List<Ref<Animal>>> getRefs(string? q, Guid? species, int page = 1)
        {
            return (await get(q, species, page))
                .Select(e => new Ref<Animal>(e))
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, Guid? species, int page = 1)
        {
            List<Animal> Animals = await get(q, species, page);
            Taxon? rootTaxon = species is null ? null :
                await _context.Taxons.FindAsync(species);
            ViewData["Page"] = page;
            ViewData["Params"] = new Dictionary<string, IEntity>
            {
                { "Вид", rootTaxon }
            };
            return View(Animals);
        }

        [HttpGet]
        public async Task<IActionResult> Show(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Species)
                .Include(a => a.Parents)
                .Include(a => a.Children)
                .FirstOrDefaultAsync(m => m.Guid == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        [HttpGet]
        public async Task<IActionResult> Crud(Guid? id)
        {
            Animal? Animal = id != null ? await _context.Animals.FindAsync(id) : null;
            ViewBag.Sex = Enum.GetValues<Sex>().Select(E => E.GetRef()).ToList();
            return View(Animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crud()
        {
            Animal Animal = new();
            var Form = Request.Form;
            if (Form["Id"] != "")
                Animal = await _context.Animals.FindAsync(new Guid(Form["Id"]));
            else
                _context.Animals.Add(Animal);
            Animal.SpeciesGuid = new Guid(Form["Species"]);
            Animal.Name = Form["Name"];
            Animal.Sex = Enum.Parse<Sex>(Form["Sex"]);
            foreach (var ParentGuid in Form["Parents"].Select(id => new Guid(id)))
                Animal.Parents.Add(await _context.Animals.FindAsync(ParentGuid));
            Animal.BirthDate = Form["BirthDate"] != "" ? DateTime.Parse(Form["BirthDate"]) : null;
            foreach (IFormFile File in Form.Files) {
                string PicturePath = $"/userfiles/{File.FileName}";
                using (FileStream stream = new(PicturePath, FileMode.Create))
                    await File.CopyToAsync(stream);
                Animal.PicturePath = PicturePath;
            }
            await _context.SaveChangesAsync();
            ViewBag.Sex = Enum.GetValues<Sex>().Select(E => E.GetRef()).ToList();
			return RedirectToAction("Show", "Animals", new { id = Animal.Guid });
        }

        // GET: Animals/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            ViewData["TaxonsGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName", animal.SpeciesGuid);
            return View(animal);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Nickname,Age,BirthDate,TaxonsGuid,Sex,Guid")] Animal animal)
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
            ViewData["TaxonsGuid"] = new SelectList(_context.Taxons, "Guid", "ScientificName", animal.SpeciesGuid);
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

            var animal = await _context.Animals
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
            var animal = await _context.Animals.FindAsync(id);
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(Guid id)
        {
            return _context.Animals.Any(e => e.Guid == id);
        }
    }
}
