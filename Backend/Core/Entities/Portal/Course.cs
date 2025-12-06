using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = null!;

        public List<User> Users { get; set; } = [];

        public string? MetaData { get; set; }
    }
}

