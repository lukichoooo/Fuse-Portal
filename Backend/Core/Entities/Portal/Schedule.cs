using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }

        [Required]
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public string? Location { get; set; }
        public string? Metadata { get; set; }
    }
}
