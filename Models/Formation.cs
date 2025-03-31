namespace Gestion_Formations.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public DateTime Date_Heure { get; set; }
        public int Prix { get; set; }
        public string Lieu { get; set; }
        public int NbPlace { get; set; }

        // Navigation property pour la relation plusieurs à plusieurs avec User
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
