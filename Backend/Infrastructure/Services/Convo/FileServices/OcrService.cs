using Core.Dtos.Settings;
using Core.Exceptions;
using Core.Interfaces.Convo.FileServices;
using IronOcr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Convo.Ocr
{
    public class IronTesseractOcrService : IOcrService
    {
        private readonly IronTesseract _orcTesseract;
        private readonly ILogger _logger;
        private readonly IronTesseractSettings _settings;

        public IronTesseractOcrService(
            IronTesseract ironTesseract,
            ILogger<IronTesseractOcrService> logger,
            IOptions<IronTesseractSettings> options
                )
        {
            _settings = options.Value;
            License.LicenseKey = _settings.LicenseKey;
            _orcTesseract = ironTesseract;
            _logger = logger;
        }

        public async Task<string> FallbackOcrAsync(Stream stream)
        {
            _logger.LogWarning("Unsupported file type, sending to OCR");
            var result = await ProcessAsync(stream);
            if (string.IsNullOrEmpty(result))
                throw new UnsupportedFileParseException("Unable to Parse type");
            return result;
        }

        public async Task<string> ProcessAsync(Stream stream)
        {

            using var ocrInput = new OcrInput();
            ocrInput.Load(stream);

            OcrResult ocrResult = await _orcTesseract.ReadAsync(ocrInput);

            return ocrResult.Text;
        }
    }
}
