using Core.Dtos;
using Core.Entities;
using Core.Interfaces.Auth;

namespace Infrastructure.Services.Auth
{
    public class AuthMapper : IAuthMapper
    {

        public User ToUser(RegisterRequest register)
            => new()
            {
                Name = register.Name,
                Email = register.Email,
                Password = register.Password,
                Address = new()
                {
                    City = register.Address.City,
                    CountryA3 = register.Address.CountryA3
                },
            };


    }
}
