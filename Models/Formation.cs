namespace Gestion_Formations.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public DateTime DateCreation { get; set; }

        // Navigation property
        public ICollection<Session> Sessions { get; set; }
    }
}
