using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Convo
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = "New Chat";

        public string? LastResponseId { get; set; }

        [Required]
        public required int UserId { get; set; }

        [Required]
        public User? User { get; set; }

        public List<Message> Messages { get; set; } = [];
    }
}
