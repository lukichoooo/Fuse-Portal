using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int? Grade { get; set; }

        public List<Schedule> Schedules { get; set; } = [];
        public List<User> Students { get; set; } = [];
        public List<User> Lecturers { get; set; } = [];
        public List<Test> Tests { get; set; } = [];

        public string? Metadata { get; set; }
    }
}
