using System.ComponentModel.DataAnnotations;
using Core.Entities.JoinTables;

namespace Core.Entities
{
    public class University : IEquatable<University>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; } = null!;

        [Required]
        public required Address Address { get; set; } = null!;

        public List<UserUniversity> UserUniversities { get; set; } = [];


        public bool Equals(University? other)
        {
            return other is not null
                && Name == other.Name
                && Address.Equals(other.Address);
        }
    }
}

