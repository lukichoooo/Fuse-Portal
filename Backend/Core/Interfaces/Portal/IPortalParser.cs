using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalParser
    {
        Task<PortalDto> ParsePortalHtml(string stringHtml);
    }
}
