using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Convo
{
    public class ChatFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = null!;

        [Required]
        public required string Text { get; set; } = null!;

        [Required]
        public required int UserId { get; set; }
        public User? User { get; set; }

        public int? MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
