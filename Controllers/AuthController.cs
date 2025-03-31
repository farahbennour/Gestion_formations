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
    using Microsoft.EntityFrameworkCore;
    using System.Text.RegularExpressions;

    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context; 


        public AuthController(IAuthService authService, IUserRepository userRepository, ApplicationDbContext context)
        {
            _authService = authService;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet("Dashboard")]
        //public IActionResult Dashboard()
        //{
        //    var user = HttpContext.User;
        //    var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        //    Console.WriteLine("Rôles de l'utilisateur : " + string.Join(", ", roles));
        //    return View();
        //}
       [Authorize(Roles = "Admin")]
[HttpGet("Dashboard")]
public IActionResult Dashboard()
{
    var dashboardData = new DashboardViewModel
    {
        FormateursCount = _context.Users.Count(u => u.Role == "Formateur"),
        ApprenantsCount = _context.Users.Count(u => u.Role == "Apprenant"),
        FormationsCount = _context.Formations.Count()
    };

    return View(dashboardData);
}

        public class DashboardViewModel
        {
            public int FormateursCount { get; set; }
            public int ApprenantsCount { get; set; }
            public int FormationsCount { get; set; }
        }

        //[HttpPost("signup")]
        //public IActionResult Signup([FromForm] RegisterModel model)
        //{
        //    if (model == null)
        //        return BadRequest("Les informations de l'utilisateur sont manquantes.");

        //    // Ajouter la validation du rôle
        //    if (string.IsNullOrEmpty(model.Role))
        //        return BadRequest("Le rôle est obligatoire.");

        //    if (string.IsNullOrEmpty(model.Username) ||
        //        string.IsNullOrEmpty(model.Email) ||
        //        string.IsNullOrEmpty(model.PasswordHash))
        //        return BadRequest("Tous les champs obligatoires doivent être remplis.");

        //    if (!IsValidEmail(model.Email))
        //        return BadRequest("L'email n'est pas valide.");

        //    var user = new User
        //    {
        //        Username = model.Username,
        //        Role = model.Role.Trim(), // Maintenant garanti de ne pas être null
        //        Email = model.Email,
        //        Telephone = model.Telephone ?? "",
        //        Adresse = model.Adresse ?? "",
        //        DateNaissance = model.DateNaissance,
        //        DateInscription = DateOnly.FromDateTime(DateTime.Today), // Date actuelle
        //        DateEmbauche = model.Role == "Formateur"
        //? model.DateEmbauche
        //: null,
        //        Specialite = model.Specialite ?? "",
        //        Experience = model.Experience ?? 0,
        //        Status = model.Role == "Formateur"
        //? (model.Status ?? "En Cours de Traitement")
        //: ""
        //    };

        //    if (!_authService.RegisterUser(user, model.PasswordHash))
        //        return BadRequest("L'utilisateur existe déjà.");

        //    return RedirectToAction("Login");
        //}

        [HttpPost("signup")]
        public IActionResult Signup([FromForm] RegisterModel model)
        {
            if (model == null)
                return BadRequest("Les informations de l'utilisateur sont manquantes.");

            if (string.IsNullOrEmpty(model.Role))
                return BadRequest("Le rôle est obligatoire.");

            if (string.IsNullOrEmpty(model.Username) ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.PasswordHash) ||
                string.IsNullOrEmpty(model.Telephone))
                return BadRequest("Tous les champs obligatoires doivent être remplis.");

            if (!IsValidEmail(model.Email))
                return BadRequest("L'email n'est pas valide.");

            // Validation du mot de passe : min 8 caractères, au moins une lettre, un chiffre et un symbole
            if (!Regex.IsMatch(model.PasswordHash, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
                return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");

            // Validation du téléphone : doit contenir exactement 8 chiffres
            if (!Regex.IsMatch(model.Telephone, @"^\d{8}$"))
                return BadRequest("Le numéro de téléphone doit contenir exactement 8 chiffres.");

            var user = new User
            {
                Username = model.Username,
                Role = model.Role.Trim(),
                Email = model.Email,
                Telephone = model.Telephone,
                Adresse = model.Adresse ?? "",
                DateNaissance = model.DateNaissance,
                DateInscription = DateOnly.FromDateTime(DateTime.Today),
                DateEmbauche = model.Role == "Formateur" ? model.DateEmbauche : null,
                Specialite = model.Specialite ?? "",
                Experience = model.Experience ?? 0,
                Status = model.Role == "Formateur" ? (model.Status ?? "En Cours de Traitement") : ""
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
            Console.WriteLine("Token");
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
        //// Dans AuthController.cs
        //[Authorize(Roles = "Admin")]
        //[HttpGet("users")]
        //public IActionResult ListUsers()
        //{
        //    var allUsers = _userRepository.GetAll().ToList();

        //    var viewModel = new UsersListViewModel
        //    {
        //        Formateurs = allUsers.Where(u => u.Role == "Formateur").ToList(),
        //        Apprenants = allUsers.Where(u => u.Role == "Apprenant").ToList()
        //    };

        //    return View(viewModel);
        //}



        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult ListUsers()
        {
            var viewModel = new UsersListViewModel
            {
                Formateurs = _context.Users
                                  .Include(u => u.Formations) // Charge les relations
                                  .Where(u => u.Role == "Formateur")
                                  .ToList(),
                Apprenants = _context.Users
                                  .Where(u => u.Role == "Apprenant")
                                  .ToList(),
                Formations = _context.Formations.ToList()
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("assign-formations")]
        public IActionResult AssignFormations(int userId, List<int> formationIds)
        {
            var user = _context.Users
                .Include(u => u.Formations)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null || user.Role != "Formateur")
            {
                return NotFound();
            }

            // Synchronisation des formations
            user.Formations = _context.Formations
                .Where(f => formationIds.Contains(f.Id))
                .ToList();

            _context.SaveChanges();
            return RedirectToAction("ListUsers");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("update-status")]
        public IActionResult UpdateStatus(int userId, string newStatus)
        {
            var user = _userRepository.GetById(userId);

            if (user == null || user.Role != "Formateur")
            {
                return NotFound();
            }

            if (newStatus == "Rejeté")
            {
                // Suppression du formateur
                _userRepository.Delete(user);
            }
            else
            {
                // Mise à jour normale
                user.Status = newStatus;

                // On ne met à jour la date d'embauche que pour le statut "Embauché"
                if (newStatus == "Embauché")
                {
                    user.DateEmbauche = DateOnly.FromDateTime(DateTime.Today);
                }

                _userRepository.Update(user);
            }

            return RedirectToAction("ListUsers");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Supprimer/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var formation = await _context.Users.FindAsync(id);
            if (formation == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteUserPartial", formation);
        }
        [HttpPost("delete-user")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

           
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok();
            
           

        }

        [Authorize]
        [HttpGet("edit-profile")]
        public IActionResult EditProfile()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            Console.WriteLine($"GET - Email récupéré : {userEmail}");

            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = _userRepository.GetByEmail(userEmail);
            if (user == null)
            {
                Console.WriteLine("GET - Utilisateur non trouvé en base");
                return NotFound();
            }

            var model = new RegisterModel
            {
                Username = user.Username,
                Email = user.Email,
                Telephone = user.Telephone,
                Adresse = user.Adresse,
                DateNaissance = user.DateNaissance,
                Role = user.Role,
                Specialite = user.Role == "Formateur" ? user.Specialite : null,
                Experience = user.Role == "Formateur" ? user.Experience : null,
                Status = user.Status
            };


            return View(model); // Vue normale pour les autres rôles
        }


        [HttpPost("edit-profile")]
        public IActionResult EditProfile(RegisterModel model)
        {
            // Vérifier si le modèle est null
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Les données du formulaire sont invalides");
                return View(new RegisterModel()); // Retourner un modèle vide ou gérer l'erreur
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = _userRepository.GetByEmail(userEmail);

            // Vérifier si l'utilisateur existe
            if (user == null)
            {
                Console.WriteLine("Utilisateur non trouvé lors de la mise à jour");
                return NotFound();
            }

            // Validation manuelle pour Formateur
            if (user.Role == "Formateur" && string.IsNullOrEmpty(model.Specialite))
            {
                ModelState.AddModelError("Specialite", "La spécialité est requise pour les formateurs");
            }
            // Vérification de la validité du mot de passe
            if (!string.IsNullOrEmpty(model.NewPassword) && !Regex.IsMatch(model.NewPassword, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                ModelState.AddModelError("NewPassword", "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");
            }



            if (ModelState.IsValid)
            {
                // Réhydrater les champs manquants pour la vue
                model.Role = user.Role;
                model.Email = user.Email;
                model.DateNaissance = user.DateNaissance; // Important pour conserver la date

                return View(model);
            }

            // Mise à jour des propriétés
            user.Username = model.Username;
            user.Telephone = model.Telephone;
            user.Adresse = model.Adresse;
            user.DateNaissance = model.DateNaissance;

            // Gestion du mot de passe
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }
            // Validation du mot de passe : min 8 caractères, au moins une lettre, un chiffre et un symbole
            if (!string.IsNullOrEmpty(model.NewPassword) && !Regex.IsMatch(model.NewPassword, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                ModelState.AddModelError("NewPassword", "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");
            }
            // Mise à jour conditionnelle pour Formateur
            if (user.Role == "Formateur")
            {
                user.Specialite = model.Specialite;
                user.Experience = model.Experience ?? 0; // Gérer les null avec une valeur par défaut
            }

            _userRepository.Update(user);

            return RedirectToAction("Index", "Home");
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("edit-profile-admin")]
        public IActionResult EditProfileAdmin()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            Console.WriteLine($"GET - Email récupéré : {userEmail}");

            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = _userRepository.GetByEmail(userEmail);
            if (user == null)
            {
                Console.WriteLine("GET - Utilisateur non trouvé en base");
                return NotFound();
            }

            var model = new RegisterModel
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash=user.PasswordHash,

            };

            
           
                return View( model); // Retourne la vue spéciale admin
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("edit-profile-admin")]
        public IActionResult EditProfileAdmin(RegisterModel model)
        {
            // Vérifier si le modèle est null
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Les données du formulaire sont invalides");
                return View(new RegisterModel()); // Retourner un modèle vide ou gérer l'erreur
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = _userRepository.GetByEmail(userEmail);

            // Vérifier si l'utilisateur existe
            if (user == null)
            {
                Console.WriteLine("Utilisateur non trouvé lors de la mise à jour");
                return NotFound();
            }

           

            if (ModelState.IsValid)
            {
                // Réhydrater les champs manquants pour la vue
                model.Role = user.Role;
                model.Email = user.Email;
                return View(model);
            }

            // Mise à jour des propriétés
           
            user.Username = model.Username;
            user.Email = model.Email;
           

            // Gestion du mot de passe
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

            _userRepository.Update(user);

            return RedirectToAction("Dashboard", "Auth");
        }





        public class UsersListViewModel
        {
            public List<User> Formateurs { get; set; }
            public List<User> Apprenants { get; set; }
            public List<Formation> Formations { get; set; } // Nouvelle propriété

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
            public string NewPassword { get; set; }
            public string Telephone { get; set; }
            public string Adresse { get; set; }
            public DateOnly? DateNaissance { get; set; }
            public DateOnly? DateInscription { get; set; }
            public DateOnly? DateEmbauche { get; set; }
            public string Role { get; set; }
            public string Specialite { get; set; }
            public int? Experience { get; set; }
            public string Status { get; set; } = "En Cours de Traitement";

        }


       
    }
}