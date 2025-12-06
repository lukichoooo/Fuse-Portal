namespace Core.Entities
{
    public class UserUniversity
    {
        public User User { get; set; } = null!;
        public int UserId { get; set; }

        public University University { get; set; } = null!;
        public int UniversityId { get; set; }
    }
}
