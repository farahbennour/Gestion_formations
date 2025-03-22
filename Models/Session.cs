namespace Gestion_Formations.Models
{
    public class Session
    {
        public int Id { get; set; }
        public DateOnly DateDebut { get; set; }
        public DateOnly DateFin { get; set; }
        public string Lieu { get; set; }

        // Foreign key to Formation
       public int FormationId { get; set; }

        public Formation Formation { get; set; }


        // Navigation property for the many-to-many relationship
        public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    }

}
