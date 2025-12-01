using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public List<Message> Messages { get; set; } = [];
    }
}
