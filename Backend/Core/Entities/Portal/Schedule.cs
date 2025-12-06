using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public string? Location { get; set; }
        public string? MetaData { get; set; }
    }
}
