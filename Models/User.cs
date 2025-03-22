using System.ComponentModel.DataAnnotations;

namespace Gestion_Formations.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string? Telephone { get; set; }
        [Required]
        public string? Adresse { get; set; }
        [Required]
        public DateOnly? DateNaissance { get; set; }
        [Required]
        public DateOnly? DateInscription { get; set; }
        [Required]
        public DateOnly? DateEmbauche { get; set; }
        [Required]
        public string Status { get; set; } = "Active";
        [Required]
        public string Role { get; set; }
        [Required]
        public string? Specialite { get; set; }
        [Required]
        public int? Experience { get; set; }

        public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();



    }
}
