namespace Gestion_Formations.Controllers
{
    using System.Security.Claims;
    using Gestion_Formations.Models;
    using Gestion_Formations.Repertoires;
    using Gestion_Formations.Service;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            var user = HttpContext.User;
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            Console.WriteLine("Rôles de l'utilisateur : " + string.Join(", ", roles));
            return View();
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromForm] RegisterModel model)
        {
            if (model == null)
                return BadRequest("Les informations de l'utilisateur sont manquantes.");

            if (string.IsNullOrEmpty(model.Username) ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.PasswordHash))
                return BadRequest("Tous les champs obligatoires doivent être remplis.");

            if (!IsValidEmail(model.Email))
                return BadRequest("L'email n'est pas valide.");

            var user = new User
            {
                Username = model.Username,
                Role = model.Role?.Trim('\r', '\n', ' '), // Nettoyage ici aussi
                Email = model.Email,
                Telephone = model.Telephone ?? "",
                Adresse = model.Adresse ?? "",
                DateNaissance = model.DateNaissance,
                DateInscription = model.DateInscription,
                DateEmbauche = model.DateEmbauche,
                Specialite = model.Specialite ?? "",
                Experience = model.Experience ?? 0,
                Status = model.Status ?? "Active"
            };

            if (!_authService.RegisterUser(user, model.PasswordHash))
                return BadRequest("L'utilisateur existe déjà.");

            return RedirectToAction("Login");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            var user = _userRepository.GetByEmail(model.Email);

            if (user == null)
                return Unauthorized("Email ou mot de passe incorrect");

            // Nettoyage du rôle
            user.Role = user.Role?.Trim('\r', '\n', ' ');
            Console.WriteLine($"Rôle nettoyé : {user.Role}");

            var token = _authService.Authenticate(model.Email, model.Password);

            if (token == null)
                return Unauthorized("Email ou mot de passe incorrect");

            HttpContext.Session.SetString("Username", user.Username);
            Console.WriteLine($"Username : {user.Username}");
            HttpContext.Session.SetString("Token", token);
            Console.WriteLine($" Token : {token}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Utilise le rôle nettoyé
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true }
            );

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Auth")
                : RedirectToAction("Index", "Home");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("Username");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) &&
                   System.Text.RegularExpressions.Regex.IsMatch(
                       email,
                       @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
                   );
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisterModel
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public string Telephone { get; set; }
            public string Adresse { get; set; }
            public DateOnly DateNaissance { get; set; }
            public DateOnly DateInscription { get; set; }
            public DateOnly DateEmbauche { get; set; }
            public string Role { get; set; }
            public string Specialite { get; set; }
            public int? Experience { get; set; }
            public string Status { get; set; } = "Active";
        }
    }
}