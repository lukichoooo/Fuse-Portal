using System.ComponentModel.DataAnnotations;
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
        public RoleType Role { get; set; } = RoleType.User;

        [Required]
        public Address Address { get; set; } = null!;

        public List<Faculty> Faculties { get; set; } = [];
        public List<University> Universities { get; set; } = [];
    }
}

