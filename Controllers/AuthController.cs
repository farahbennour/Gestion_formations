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

        [HttpPost("signup")]
        public IActionResult Signup([FromForm] RegisterModel model)
        {
            Console.WriteLine(" [Signup] Début de la méthode Signup.");

            if (model == null)
            {
                Console.WriteLine(" [Signup] Le modèle est null.");
                ModelState.AddModelError(string.Empty, "Les informations de l'utilisateur sont manquantes.");
                return View(model);
            }

            Console.WriteLine(" [Signup] Modèle reçu pour l'utilisateur : " + model.Username);

            

            if (string.IsNullOrEmpty(model.Username))
            {
                Console.WriteLine(" [Signup] Le nom d'utilisateur est manquant.");
                ModelState.AddModelError(nameof(model.Username), "Le nom d'utilisateur est obligatoire.");
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                Console.WriteLine(" [Signup] L'email est manquant.");
                ModelState.AddModelError(nameof(model.Email), "L'email est obligatoire.");
            }

            if (string.IsNullOrEmpty(model.PasswordHash))
            {
                Console.WriteLine(" [Signup] Le mot de passe est manquant.");
                ModelState.AddModelError(nameof(model.PasswordHash), "Le mot de passe est obligatoire.");
            }

            if (string.IsNullOrEmpty(model.Telephone))
            {
                Console.WriteLine(" [Signup] Le numéro de téléphone est manquant.");
                ModelState.AddModelError(nameof(model.Telephone), "Le numéro de téléphone est obligatoire.");
            }

            if (!IsValidEmail(model.Email))
            {
                Console.WriteLine(" [Signup] L'email n'est pas valide : " + model.Email);
                ModelState.AddModelError(nameof(model.Email), "L'email n'est pas valide.");
            }

            
            if (!Regex.IsMatch(model.PasswordHash, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                Console.WriteLine("❌ [Signup] Le mot de passe ne respecte pas les règles de sécurité.");
                ModelState.AddModelError(nameof(model.PasswordHash), "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");
            }

          
            if (!Regex.IsMatch(model.Telephone, @"^\d{8}$"))
            {
                Console.WriteLine("❌ [Signup] Le numéro de téléphone est invalide : " + model.Telephone);
                ModelState.AddModelError(nameof(model.Telephone), "Le numéro de téléphone doit contenir exactement 8 chiffres.");
            }
            if (model.Role != "Formateur")
            {
                ModelState.Remove(nameof(model.Specialite));
            }

         


            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ [Signup] ModelState invalide, erreurs relevées :");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($" - {state.Key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

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

            Console.WriteLine(" [Signup] Création de l'utilisateur : " + user.Username);

            if (!_authService.RegisterUser(user, model.PasswordHash))
            {
                Console.WriteLine("[Signup] L'utilisateur existe déjà.");
                ModelState.AddModelError(string.Empty, "L'utilisateur existe déjà.");
                return View(model);
            }

            Console.WriteLine(" [Signup] Utilisateur enregistré avec succès.");
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
                new Claim(ClaimTypes.Role, user.Role) };

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
   



        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult ListUsers()
        {
            var viewModel = new UsersListViewModel
            {
                Formateurs = _context.Users
                                  .Include(u => u.Formations)
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
             
                _userRepository.Delete(user);
            }
            else
            {
             
                user.Status = newStatus;

              
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
                //DateNaissance = user.DateNaissance,
                Role = user.Role,
                Specialite = user.Role == "Formateur" ? user.Specialite : null,
                Experience = user.Role == "Formateur" ? user.Experience : null,
                Status = user.Status
            };


            return View(model); 
        }



        [HttpPost("edit-profile")]
        public IActionResult EditProfile(RegisterModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Les données du formulaire sont invalides.");
                return View(new RegisterModel());
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = _userRepository.GetByEmail(userEmail);

            if (user == null)
            {
                Console.WriteLine("Utilisateur non trouvé.");
                return NotFound();
            }

            if (!Regex.IsMatch(model.Telephone, @"^\d{8}$"))
            {
                Console.WriteLine("❌ [Signup] Le numéro de téléphone est invalide : " + model.Telephone);
                ModelState.AddModelError(nameof(model.Telephone), "Le numéro de téléphone doit contenir exactement 8 chiffres.");
            }
          

            if (user.Role != "Formateur")
            {
                ModelState.Remove(nameof(model.Specialite));
            }

            ModelState.Remove(nameof(model.PasswordHash));

            Console.WriteLine(" ---- DÉBUG ModelState ----");
            foreach (var state in ModelState)
            {
                Console.WriteLine($"Clé : {state.Key}, Erreurs : {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
            }
            Console.WriteLine(" --------------------------");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState invalide, retour à la vue.");
                model.Role = user.Role;
                model.Email = user.Email;
                return View(model);
            }

          
            user.Username = model.Username;
            user.Telephone = model.Telephone;
            user.Adresse = model.Adresse;

           
   
            if (user.Role == "Formateur")
            {
                user.Specialite = model.Specialite;
                user.Experience = model.Experience ?? 0;
            }

            _userRepository.Update(user);
            Console.WriteLine(" Utilisateur mis à jour avec succès !");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            Console.WriteLine($"GET - Email récupéré : {userEmail}");

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = _userRepository.GetByEmail(userEmail);
            if (user == null)
            {
                Console.WriteLine("GET - Utilisateur non trouvé en base");
                return NotFound();
            }

            var model = new ChangePasswordModel();
            return View(model); // Retourne la vue du formulaire de changement de mot de passe
        }


        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Les données du formulaire sont invalides.");
                return View(model);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = _userRepository.GetByEmail(userEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Utilisateur non trouvé.");
                return View(model);
            }

            // Vérification de l'ancien mot de passe (si requis)
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.OldPassword), "L'ancien mot de passe est incorrect.");
                return View(model);
            }

            // Vérification que le nouveau mot de passe est bien fourni
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le nouveau mot de passe est requis.");
                return View(model);
            }

            // Validation du nouveau mot de passe
            if (!Regex.IsMatch(model.NewPassword, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");
                return View(model);
            }

            // Vérifier si l'ancien et le nouveau mot de passe sont identiques
            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le nouveau mot de passe doit être différent de l'ancien.");
                return View(model);
            }

            // Hachage du nouveau mot de passe
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userRepository.Update(user);
            Console.WriteLine(" Mot de passe mis à jour avec succès !");
        
            TempData["SuccessMessage"] = "Mot de passe mis à jour avec succès.";
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("edit-profile-admin")]
        public IActionResult EditProfileAdmin(ChangePasswordModel model)
        {
             if (model == null)
            {
                ModelState.AddModelError(string.Empty, "Les données du formulaire sont invalides.");
                return View(model);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = _userRepository.GetByEmail(userEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Utilisateur non trouvé.");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.OldPassword), "L'ancien mot de passe est incorrect.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le nouveau mot de passe est requis.");
                return View(model);
            }

         
            if (!Regex.IsMatch(model.NewPassword, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le mot de passe doit contenir au moins 8 caractères, une lettre, un chiffre et un symbole.");
                return View(model);
            }

       
            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "Le nouveau mot de passe doit être différent de l'ancien.");
                return View(model);
            }

         
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userRepository.Update(user);
            Console.WriteLine(" Mot de passe mis à jour avec succès !");

           
            TempData["SuccessMessage"] = "Mot de passe mis à jour avec succès.";
            return RedirectToAction("Dashboard", "Auth");
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

            var model = new ChangePasswordModel
            {

                NewPassword = user.PasswordHash,

            };
            return View(model);

        }



        public class DashboardViewModel
        {
            public int FormateursCount { get; set; }
            public int ApprenantsCount { get; set; }
            public int FormationsCount { get; set; }
        }

        public class UsersListViewModel
        {
            public List<User> Formateurs { get; set; }
            public List<User> Apprenants { get; set; }
            public List<Formation> Formations { get; set; } 

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
            //public string NewPassword { get; set; }
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


        public class ChangePasswordModel
        {
           
           public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }



    }
}