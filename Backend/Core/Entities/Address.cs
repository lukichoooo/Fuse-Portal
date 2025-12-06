using System.ComponentModel.DataAnnotations;
using Core.Dtos;
using Core.Enums;

namespace Core.Entities
{
    public class Address : IEquatable<Address>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required CountryCode CountryA3 { get; set; }

        [Required]
        public required string City { get; set; } = null!;


        public bool Equals(Address? other)
        {
            return other is not null
                && other.City == City
                && other.CountryA3 == CountryA3;
        }

        public static implicit operator Address(AddressDto dto)
            => new()
            {
                CountryA3 = dto.CountryA3,
                City = dto.City
            };
    }
}
