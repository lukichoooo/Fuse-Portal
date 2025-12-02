using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Convo
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? LastResponseId { get; set; }

        public List<Message> Messages { get; set; } = [];
    }
}
