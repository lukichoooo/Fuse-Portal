using Core.Dtos;
using Core.Dtos.Settings;
using Core.Exceptions;
using Core.Interfaces.Convo.FileServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Convo.FileServices
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IOcrService _ocr;
        private readonly FileProcessingSettings _settings;
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
            _settings = options.Value;

            _handlers = [];

            foreach (var kv in _settings.Handlers)
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
                ValidateSize(ms.Length, f.Name);

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


        // Helper

        private void ValidateSize(long fileSize, string fileName)
        {
            if (fileSize > _settings.MaxFileSizeBytes)
            {
                _logger.LogWarning(
                    "File too big. Name={Name}, SizeKB={SizeKb}, MaxKB={MaxKb}",
                    fileName,
                    fileSize / 1024,
                    _settings.MaxFileSizeBytes / 1024
                );

                throw new FileTooLargeException(
                    $"File '{fileName}' is {fileSize / 1024} KB, exceeding the max allowed {_settings.MaxFileSizeBytes / 1024} KB."
                );
            }
        }

    }
}
