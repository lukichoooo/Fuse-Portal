using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = null!;

        [Required]
        public required string Content { get; set; } = null!;

        [Required]
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }


        public DateTime? Date { get; set; }

        public string? Metadata { get; set; }
    }
}
