using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class ChatFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public int MessageId { get; set; }

        [Required]
        public Message Message { get; set; } = null!;
    }
}
