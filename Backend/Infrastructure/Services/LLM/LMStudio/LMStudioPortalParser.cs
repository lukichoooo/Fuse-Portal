using Core.Dtos;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioPortalParser(
            ILMStudioMapper mapper
            ) : IPortalParser
    {
        private readonly ILMStudioMapper _mapper = mapper;

        public Task<PortalDto> ParsePortalHtml(string stringHtml)
        {
            throw new NotImplementedException();
        }
    }
}
