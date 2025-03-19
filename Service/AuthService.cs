namespace Gestion_Formations.Service
{
    using Gestion_Formations.Models;
    using Gestion_Formations.Repertoires;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _config = configuration;
        }

        // Inscription d'un nouvel utilisateur avec hashage du mot de passe
        public bool RegisterUser(User user, string password)
        {
            if (_userRepository.GetByEmail(user.Email) != null)
            {
                return false; // L'utilisateur existe déjà
            }

            // Hashage du mot de passe avec BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _userRepository.Add(user);
            return true;
        }


        // Méthode d'authentification de l'utilisateur avec l'email et le mot de passe
        //public string Authenticate(string email, string password)
        //{
        //    var user = _userRepository.GetByEmail(email);

        //    if (user == null)
        //    {
        //        Console.WriteLine("Utilisateur non trouvé"); // Debug log
        //        return null; // L'utilisateur n'existe pas
        //    }

        //    // Vérifie que le mot de passe haché correspond à celui stocké
        //    if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        //    {
        //        Console.WriteLine("Mot de passe incorrect"); // Debug log
        //        return null; // Mot de passe incorrect
        //    }

        //    // Si tout est bon, génère le token JWT
        //    return GenerateJwtToken(user);
        //}
        public string Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null; // Assure-toi de renvoyer null si l'email ou le mot de passe sont vides
            }

            var user = _userRepository.GetByEmail(email);
            if (user == null)
                return null; // L'utilisateur n'existe pas

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null; // Mot de passe incorrect

            var token = GenerateJwtToken(user);
            return token;
        }



        // Génération du JWT
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),  // Inclure l'email
        new Claim(ClaimTypes.Role, user.Role)  // Le rôle de l'utilisateur
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
