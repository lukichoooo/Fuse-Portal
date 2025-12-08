using Core.Dtos;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class PortalParser() : IPortalParser
    {
        public Task<PortalParserDto> ParsePortalHtml(ParsePortalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
