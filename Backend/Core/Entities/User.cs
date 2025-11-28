using Core.Enums;

namespace Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public RoleType Role { get; set; } = RoleType.User;

        public List<Faculty> Faculties { get; set; } = [];
        public List<University> Universities { get; set; } = [];
    }
}

