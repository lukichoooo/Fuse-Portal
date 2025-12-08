using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalParser
    {
        Task<PortalParserDto> ParsePortalHtml(ParsePortalRequest request);
    }
}
