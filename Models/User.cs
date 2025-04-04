﻿using System.ComponentModel.DataAnnotations;

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
        [StringLength(8)]
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
        public string? Status { get; set; } = "En Cours de Traitelment";

        [Required]
        public string Role { get; set; }

        [Required]
        public string? Specialite { get; set; }

        [Required]
        public int? Experience { get; set; }

        // Navigation property pour la relation plusieurs à plusieurs avec Formation
        public ICollection<Formation> Formations { get; set; } = new List<Formation>();
    }
}
