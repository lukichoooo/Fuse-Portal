using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class University : IEquatable<University>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public Address Address { get; set; } = null!;

        public List<User> Users { get; set; } = [];

        public bool Equals(University? other)
        {
            return other is not null
                && Name == other.Name
                && Address.Equals(other.Address);
        }
    }
}

