using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Faculty
    {
        [Key]
        public string Name { get; set; }

        public List<User> Users { get; set; } = [];
    }
}
