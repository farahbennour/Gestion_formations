using Gestion_Formations.Models;
using Gestion_Formations.Repertoires;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("formations")]
  // Vérification du rôle directement ici
public class FormationsController : Controller
{
    private readonly IFormationRepository _formationRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public FormationsController(IFormationRepository formationRepository, IHttpClientFactory httpClientFactory)
    {
        _formationRepository = formationRepository;
        _httpClientFactory = httpClientFactory;
    }

    // GET: formations/create
    [HttpGet("create")]
    public IActionResult Create()
    {
        var token = HttpContext.Session.GetString("Token");
       // Récupère le token de la session
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");  // Redirige vers la page de login si le token est absent
        }

        // Validez le token ici si nécessaire (par exemple en appelant une API pour vérifier le token)

        return View();
    }

    // POST: formations/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] Formation model)
    {
        if (model == null)
        {
            return BadRequest("Les informations de la formation sont manquantes.");
        }

        if (string.IsNullOrEmpty(model.Nom) || string.IsNullOrEmpty(model.Description))
        {
            return BadRequest("Tous les champs obligatoires doivent être remplis.");
        }

        // Vérifier que l'utilisateur a le rôle Admin à partir du token JWT
        var token = HttpContext.Session.GetString("Token");
        Console.WriteLine($"[LOGOUT] Token trouvé : {token}");// Récupère le token de la session
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");  // Si le token est absent, redirige vers login
        }

        var userRole = GetUserRoleFromToken(token); // Décodez ou validez le rôle à partir du token JWT
        if (userRole != "Admin")
        {
            return Forbid(); // Si l'utilisateur n'a pas le rôle "Admin", on lui interdit l'accès
        }

        // Créer la formation dans la base de données
        _formationRepository.AddFormation(model);
        await _formationRepository.SaveChangesAsync();

        return RedirectToAction(nameof(Index));  // Redirige vers la liste des formations après la création
    }

    private string GetUserRoleFromToken(string token)
    {
        // Logique pour décoder le token JWT et récupérer le rôle
        // Vous pouvez utiliser une bibliothèque comme `System.IdentityModel.Tokens.Jwt` pour décoder le token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var role = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return role;
    }

    // GET: formations
    [HttpGet("index")]
    public async Task<IActionResult> Index()
    {
        var formations = await _formationRepository.GetAllFormationsAsync();
        return View(formations);  // Retourne la liste des formations à la vue
    }
}
