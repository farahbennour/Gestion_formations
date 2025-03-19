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

       
        public bool RegisterUser(User user, string password)
        {
            if (_userRepository.GetByEmail(user.Email) != null)
            {
                return false; 
            }

            // Hashage du mot de passe avec BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _userRepository.Add(user);
            return true;
        }


        public string Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null; 
            }

            var user = _userRepository.GetByEmail(email);
            if (user == null)
                return null; 

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null; 

            var token = GenerateJwtToken(user);
            return token;
        }
        public bool VerifyPassword(User user, string password)
        {
            
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return false;
            }

           
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }



        // Génération du JWT
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),  
        new Claim(ClaimTypes.Role, user.Role)  
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
