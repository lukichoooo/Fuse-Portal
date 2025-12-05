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

        [Required]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;

        public List<Message> Messages { get; set; } = [];
    }
}
