using Gestion_Formations.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Gestion_Formations.Models;

namespace Gestion_Formations.Controllers
{
    [Route("formations")]
    public class FormationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // GET: Formation
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Formation/Index.cshtml");
        }

        [HttpGet("create")]
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Formation/Create
        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nom,Description,DateCreation")] Formation formation)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(formation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Formation/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }
            return View(formation);
        }

        // POST: Formation/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Duree")] Formation formation)
        {
            if (id != formation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormationExists(formation.Id))
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
            return View(formation);
        }

        // GET: Formation/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formation = await _context.Formations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formation == null)
            {
                return NotFound();
            }

            return View(formation);
        }

        // POST: Formation/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            _context.Formations.Remove(formation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormationExists(int id)
        {
            return _context.Formations.Any(e => e.Id == id);
        }
    }
}