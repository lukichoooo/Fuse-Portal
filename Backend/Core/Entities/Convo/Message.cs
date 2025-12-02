using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Convo
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; } = "";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int ChatId { get; set; }

        public Chat Chat { get; set; } = null!;

        public List<ChatFile> Files = [];
    }
}

