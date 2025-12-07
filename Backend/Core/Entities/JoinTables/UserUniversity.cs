namespace Core.Entities.JoinTables
{
    public class UserUniversity
    {
        public User? User { get; set; }
        public required int UserId { get; set; }

        public University? University { get; set; }
        public required int UniversityId { get; set; }
    }
}
