using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class University
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public List<User> Users { get; set; } = [];
    }
}

