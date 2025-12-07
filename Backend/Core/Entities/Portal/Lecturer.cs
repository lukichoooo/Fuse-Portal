using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Portal
{
    public class Lecturer
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
