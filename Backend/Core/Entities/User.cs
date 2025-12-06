using System.ComponentModel.DataAnnotations;
using Core.Entities.Convo;
using Core.Entities.Portal;
using Core.Enums;

namespace Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public RoleType Role { get; set; } = RoleType.Student;

        [Required]
        public Address Address { get; set; } = null!;

        public List<UserUniversity> UserUniversities { get; set; } = [];

        public List<Subject> SubjectEnrollments { get; set; } = [];
        public List<Subject> TeachingSubjects { get; set; } = [];
        public List<Course> Courses { get; set; } = [];
        public List<Chat> Chats { get; set; } = [];
    }
}

