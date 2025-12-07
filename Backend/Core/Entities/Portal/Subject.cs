using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = null!;

        [Required]
        public required int UserId { get; set; }
        public User? User { get; set; }

        public int? Grade { get; set; }

        public string? Metadata { get; set; }

        public List<Schedule> Schedules { get; set; } = [];
        public List<Lecturer> Lecturers { get; set; } = [];
        public List<Test> Tests { get; set; } = [];
    }
}

