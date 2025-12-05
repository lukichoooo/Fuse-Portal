using Core.Interfaces.Auth;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Auth
{
    public class CurrentContext(IHttpContextAccessor httpContextAccessor) : ICurrentContext
    {
        public int GetCurrentUserId()
            => int.Parse(httpContextAccessor.HttpContext!.User.FindFirst("id")!.Value);
    }
}
