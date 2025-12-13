using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Questions { get; set; } = null!;

        public string Answers { get; set; } = "";

        public string? Results { get; set; }

        public int? ScoreFrom100 { get; set; }

        [Required]
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
