using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class ChatFile
    {
        [Key]
        public int Id { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        [Required]
        public int MessageId { get; set; }

        [Required]
        public Message Message { get; set; } = null!;
    }
}
