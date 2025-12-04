using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.Convo.FileServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Convo.FileServices
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IOcrService _ocr;
        private readonly ILogger<FileProcessingService> _logger;
        private readonly IFileTextParser _fileParser;
        private readonly Dictionary<string, Func<Stream, Task<string>>> _handlers;

        public FileProcessingService(
            IOcrService ocr,
            ILogger<FileProcessingService> logger,
            IOptions<FileProcessingSettings> options,
            IFileTextParser fileParser
            )
        {
            _ocr = ocr;
            _logger = logger;
            _fileParser = fileParser;

            _handlers = [];

            foreach (var kv in options.Value.Handlers)
            {
                var ext = kv.Key;
                var handlerType = kv.Value;

                _handlers[ext] = handlerType switch
                {
                    "text" => _fileParser.ReadTextAsync,
                    "docx" => _fileParser.ReadDocxAsync,
                    "ocr" => _ocr.ProcessAsync,
                    _ => _ocr.FallbackOcrAsync
                };
            }

        }

        public async Task<List<FileDto>> ProcessFilesAsync(List<FileUpload> files)
        {
            var tasks = files.Select(async f =>
            {
                await using var ms = new MemoryStream();
                await f.Stream.CopyToAsync(ms);
                ms.Position = 0;

                string ext = Path.GetExtension(f.Name).ToLowerInvariant();
                string text = await (_handlers.TryGetValue(ext, out var handler)
                                        ? handler(ms)
                                        : _ocr.FallbackOcrAsync(ms));

                _logger.LogInformation("FileName: {}, Contents: {}", f.Name, text);
                return new FileDto(f.Name, text);
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

    }
}
