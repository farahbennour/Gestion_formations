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
    [Authorize(Roles = "Admin")]
    public class FormationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("List")]
        [Authorize(Roles = "Admin")]
        // GET: Formation
        //public async Task<IActionResult> Index()
        //{
        //    return View("~/Views/Formation/Index.cshtml");
        //}
        public IActionResult List()
        {
            var formations = _context.Formations.Include(f => f.Users) // Inclure les utilisateurs liés
        .ToList(); // Assurez-vous que _context n'est pas null
            return View(formations);
        }


        [HttpGet("create")]
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {
            return PartialView("create");
        }

        // POST: Formation/Create
        [HttpPost("create")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nom,Description,Date_Heure,Prix,Lieu")] Formation formation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formation);
                await _context.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View();
        }

        //// GET: Formation/Edit/5
        //[Authorize(Roles = "Admin")]
        //[HttpGet("Modifier/{id}")]
        //public IActionResult Modifier(int id)
        //{
        //    var formation = _context.Formations.FirstOrDefault(f => f.Id == id);
        //    if (formation == null)
        //    {
        //        return NotFound();
        //    }

        //    return PartialView("_EditForm", formation);  // Retourne une vue partielle
        //}


        ////// POST: Formation/Edit/5

        //[HttpPost("Edit/{id}")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Description,Date_Heure")] Formation formation)
        //{
        //    Console.WriteLine($"ID reçu : {id}");

        //    if (id != formation.Id)
        //    {
        //        Console.WriteLine("ID dans l'URL ne correspond pas à l'ID du formulaire.");
        //        return BadRequest("ID mismatch");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        Console.WriteLine("ModelState invalide : " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        //        return BadRequest("Validation error");
        //    }

        //    var existingFormation = await _context.Formations.FindAsync(id);
        //    if (existingFormation == null)
        //    {
        //        Console.WriteLine("Formation non trouvée.");
        //        return NotFound();
        //    }

        //    existingFormation.Nom = formation.Nom;
        //    existingFormation.Description = formation.Description;
        //    existingFormation.Date_Heure = formation.Date_Heure;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("List");
        //}


        [HttpGet("Modifier/{id}")]
        public IActionResult Modifier(int id)
        {
            var formation = _context.Formations
                .Include(f => f.Users)
                .FirstOrDefault(f => f.Id == id);

            if (formation == null) return NotFound();

            ViewBag.Formateurs = _context.Users
                .Where(u => u.Role == "Formateur")
                .ToList();

            return PartialView("_EditForm", formation);
        }

        [HttpPost("Modifier/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Nom,Description,Date_Heure,Prix,Lieu")] Formation formation,
            List<int> selectedFormateurs)
        {
            var existingFormation = await _context.Formations
                .Include(f => f.Users)
                .FirstOrDefaultAsync(f => f.Id == id);

            // Mise à jour des propriétés de base
            existingFormation.Nom = formation.Nom;
            existingFormation.Description = formation.Description;
            existingFormation.Date_Heure = formation.Date_Heure;
            existingFormation.Prix = formation.Prix;
            existingFormation.Lieu = formation.Lieu;

            // Gestion des formateurs
            var selectedIds = selectedFormateurs ?? new List<int>();
            var currentIds = existingFormation.Users.Select(u => u.Id).ToList();

            //// Supprimer les formateurs désélectionnés
            //foreach (var user in existingFormation.Users.Where(u => !selectedIds.Contains(u.Id)).ToList())
            //{
            //    existingFormation.Users.Remove(user);
            //}

            // Ajouter les nouveaux formateurs
            foreach (var userId in selectedIds.Where(id => !currentIds.Contains(id)))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null) existingFormation.Users.Add(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var formation = await _context.Formations.Include(f => f.Users)
                .FirstOrDefaultAsync(f => f.Id == id); ;
            if (formation == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", formation);
        }

        [HttpGet("Supprimer/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }
            return PartialView("_DeletePartial", formation);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var formation = await _context.Formations.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }

            try
            {
                _context.Formations.Remove(formation);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }

        //[HttpGet("delete")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var formation = await _context.Formations
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (formation == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(formation);
        //}

        //[HttpPost("delete/{id}")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var formation = await _context.Formations.FindAsync(id);
        //    if (formation == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Formations.Remove(formation);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(List));

        //}


        private bool FormationExists(int id)
        {
            return _context.Formations.Any(e => e.Id == id);
        }
     
    }
}

