using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public List<User> Users { get; set; } = [];
    }
}

