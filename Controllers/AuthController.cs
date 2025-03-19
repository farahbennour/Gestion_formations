//namespace Gestion_Formations.Controllers
//{
//    using Gestion_Formations.Models;
//    using Gestion_Formations.Repertoires;
//    using Gestion_Formations.Service;
//    using Microsoft.AspNetCore.Mvc;
//    using System.Net.Mail;

//    [Route("auth")]
//    public class AuthController : Controller
//    {
//        private readonly IAuthService _authService;
//        private readonly IUserRepository _userRepository;

//        public AuthController(IAuthService authService, IUserRepository userRepository)
//        {
//            _authService = authService;
//            _userRepository = userRepository;
//        }


//        // Affiche la vue de l'inscription
//        [HttpGet("signup")]
//        public IActionResult Signup()
//        {
//            return View();  // Affiche la page Signup.cshtml
//        }

//        [HttpGet("Dashboard")]
//        public IActionResult Dashboard()
//        {
//            return View();  // Affiche la page Signup.cshtml
//        }
//        // Gère le formulaire d'inscription
//        [HttpPost("signup")]
//        public IActionResult Signup([FromBody] RegisterModel model)
//        {
//            Console.WriteLine($"Email reçu: {model.Email}");  // Log pour vérifier l'email

//            if (!IsValidEmail(model.Email))
//            {
//                return BadRequest("L'email n'est pas valide.");
//            }

//            var user = new User
//            {
//                Username = model.Username,
//                Role = model.Role,
//                Email = model.Email
//            };

//            var result = _authService.RegisterUser(user, model.Password);

//            if (!result)
//                return BadRequest("L'utilisateur existe déjà.");

//            return RedirectToAction("Login");
//        }


//        // Méthode pour vérifier si l'email est valide
//        private bool IsValidEmail(string email)
//        {
//            if (string.IsNullOrEmpty(email))
//            {
//                return false; // L'email est vide ou null, retourne false
//            }

//            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
//            return emailRegex.IsMatch(email);
//        }


//        // Affiche la vue de connexion
//        [HttpGet("login")]
//        public IActionResult Login()
//        {
//            return View();  // Affiche la page Login.cshtml
//        }

//        // Gère le formulaire de connexion
//        //    [HttpPost("login")]
//        //    public IActionResult Login([FromBody] LoginModel model)
//        //    {
//        //        if (model == null)
//        //        {
//        //            return BadRequest("Les informations de connexion sont manquantes.");
//        //        }

//        //        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
//        //        {
//        //            return BadRequest("L'email et le mot de passe sont requis.");
//        //        }

//        //        Console.WriteLine($"Email: {model.Email}");  // Affiche l'email pour vérifier

//        //        var token = _authService.Authenticate(model.Email, model.Password);

//        //        if (token == null)
//        //            return Unauthorized("Email ou mot de passe incorrect");

//        //        Console.WriteLine($"Token généré : {token}");

//        //        // Récupérer l'utilisateur (pour vérifier son rôle, par exemple)
//        //        var user = _userRepository.GetByEmail(model.Email);

//        //        if (user == null)
//        //            return Unauthorized("Utilisateur non trouvé");

//        //        // Exemple : Rediriger vers le Dashboard en fonction du rôle
//        //        if (user.Role == "Admin")
//        //            return RedirectToAction("Dashboard", "Home"); // Redirige vers le Dashboard

//        //        return RedirectToAction("Index", "Home");  // Redirige vers la page d'accueil (ou autre)
//        //    }
//        //}
//        [HttpPost("login")]
//        public IActionResult Login([FromBody] LoginModel model)
//        {
//            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
//            {
//                return BadRequest("Email et mot de passe sont requis.");
//            }

//            Console.WriteLine($"Email: {model.Email}");

//            var token = _authService.Authenticate(model.Email, model.Password);

//            if (token == null)
//            {
//                return Unauthorized("Email ou mot de passe incorrect");
//            }

//            Console.WriteLine($"Token généré : {token}");

//            var user = _userRepository.GetByEmail(model.Email);

//            if (user == null)
//            {
//                return Unauthorized("Utilisateur non trouvé");
//            }

//            if (string.IsNullOrEmpty(user.Role))
//            {
//                return Unauthorized("Rôle non défini pour cet utilisateur");
//            }

//            // Redirection basée sur le rôle
//            if (user.Role == "Admin")
//                return RedirectToAction("Dashboard"); // Exemple pour admin

//            return RedirectToAction("Index", "Home"); // Autre redirection pour les autres rôles
//        }
//    }
//        public class LoginModel
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//    }

//    public class RegisterModel
//    {
//        public string Username { get; set; }
//        public string Password { get; set; }
//        public string Role { get; set; } // "Apprenant" ou "Formateur"
//        public string Email { get; set; }
//    }
//}


namespace Gestion_Formations.Controllers
{
    using Gestion_Formations.Models;
    using Gestion_Formations.Repertoires;
    using Gestion_Formations.Service;
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

        // Affiche la vue de l'inscription
        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return View();  // Affiche la page Signup.cshtml
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();  // Affiche la page Dashboard.cshtml
        }

        // Gère le formulaire d'inscription
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] RegisterModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Les informations de l'utilisateur sont manquantes.");
            }

            if (!IsValidEmail(model.Email))
            {
                return BadRequest("L'email n'est pas valide.");
            }

            var user = new User
            {
                Username = model.Username,
                Role = model.Role,
                Email = model.Email
            };

            var result = _authService.RegisterUser(user, model.Password);

            if (!result)
                return BadRequest("L'utilisateur existe déjà.");

            return RedirectToAction("Login");
        }

        // Vérifie si l'email est valide
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        // Affiche la vue de connexion
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();  // Affiche la page Login.cshtml
        }

        // Gère le formulaire de connexion
        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email et mot de passe sont requis.");
            }

            // Authentification de l'utilisateur
            var token = _authService.Authenticate(model.Email, model.Password);

            if (token == null)
            {
                return Unauthorized("Email ou mot de passe incorrect");
            }

            var user = _userRepository.GetByEmail(model.Email);

            if (user == null)
            {
                return Unauthorized("Utilisateur non trouvé");
            }

            // Gérer la redirection en fonction du rôle
            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Auth");  // Redirige vers la page Dashboard pour l'admin
            }
            else
            {
                return RedirectToAction("Index", "Home");  // Redirige vers la page d'accueil pour les autres utilisateurs
            }
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // "Apprenant" ou "Formateur"
        public string Email { get; set; }
    }
}
