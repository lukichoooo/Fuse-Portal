using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Auth
{
    public interface IAuthMapper
    {
        public User ToUser(RegisterRequest register);
    }
}
