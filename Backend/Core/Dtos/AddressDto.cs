using Core.Entities;
using Core.Enums;

namespace Core.Dtos
{
    public class AddressDto(string City, CountryCode CountryA3)
    {
        public string City { get; set; } = City;
        public CountryCode CountryA3 { get; set; } = CountryA3;

        public static implicit operator AddressDto(Address address)
            => new(address.City, address.CountryA3);
    }
}
