using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;


        public int? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public string? Metadata { get; set; }
    }
}
