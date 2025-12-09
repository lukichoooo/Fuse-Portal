using Core.Dtos;

namespace Core.Interfaces.Portal
{
    public interface IPortalParser
    {
        Task<PortalParserResponseDto> ParsePortalHtml(PortalParserRequestDto request);
    }
}
