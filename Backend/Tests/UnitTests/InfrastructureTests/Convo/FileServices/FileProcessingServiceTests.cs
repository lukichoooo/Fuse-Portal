using System.Text;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Interfaces.Convo.FileServices;
using Infrastructure.Services.Convo.FileServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.Convo.FileServices
{
    [TestFixture]
    public class FileProcessingServiceTests
    {
        private readonly FileProcessingSettings _fileProcessingSettings = new()
        {
            Handlers = {
                {".txt", "text"},
                {".md", "text"},
                {".docx", "docx"},
                {".jpeg", "ocr"},
                {".png", "ocr"},
                {".pdf", "ocr"}
            }
        };

        private Mock<IOcrService> _ocrMock = null!;
        private Mock<IOptions<FileProcessingSettings>> _optionsMock = null!;
        private Mock<ILogger<FileProcessingService>> _loggerMock = null!;
        private Mock<IFileTextParser> _fileParserMock = null!;

        private IFileProcessingService CreateSUT()
            => new FileProcessingService(
                _ocrMock.Object,
                _loggerMock.Object,
                _optionsMock.Object,
                _fileParserMock.Object
            );


        [SetUp]
        public void BeforeEach()
        {
            _ocrMock = new Mock<IOcrService>();
            _loggerMock = new Mock<ILogger<FileProcessingService>>();
            _fileParserMock = new Mock<IFileTextParser>();

            _optionsMock = new Mock<IOptions<FileProcessingSettings>>();
            _optionsMock.Setup(x => x.Value).Returns(_fileProcessingSettings);
        }

        private void VerifyCalled(
                bool isText = false,
                bool isDocx = false,
                bool isOcrProcess = false,
                bool isOcrFallback = false
                )
        {
            _fileParserMock.Verify(fp => fp.ReadTextAsync(It.IsAny<Stream>()),
                    isText ? Times.Once() : Times.Never());
            _fileParserMock.Verify(fp => fp.ReadDocxAsync(It.IsAny<Stream>()),
                    isDocx ? Times.Once() : Times.Never());
            _ocrMock.Verify(o => o.ProcessAsync(It.IsAny<Stream>()),
                    isOcrProcess ? Times.Once() : Times.Never());
            _ocrMock.Verify(o => o.FallbackOcrAsync(It.IsAny<Stream>()),
                    isOcrFallback ? Times.Once() : Times.Never());
        }

        private FileUpload CreateFileUpload(string fileName, string content = "dummy") =>
            new(fileName, new MemoryStream(Encoding.UTF8.GetBytes(content)));

        [TestCase(".pdf")]
        [TestCase(".png")]
        [TestCase(".jpeg")]
        public async Task ProcessFilesAsync_OCR_Success(string extension)
        {
            const string expected = "OCR_TEXT";
            _ocrMock.Setup(x => x.ProcessAsync(It.IsAny<Stream>()))
                    .ReturnsAsync(expected);

            var sut = CreateSUT();
            var fileName = "NAME" + extension;

            var fileUpload = CreateFileUpload("NAME" + extension);


            var result = await sut.ProcessFilesAsync([fileUpload]);

            Assert.That(result[0].Text, Is.EqualTo(expected));
            VerifyCalled(isOcrProcess: true);
        }

        [TestCase(".bruhhsadiadi")]
        public async Task ProcessFilesAsync_OCR_FALLBACK_Success(string extension)
        {
            const string expected = "FALLBACK";
            _ocrMock.Setup(x => x.FallbackOcrAsync(It.IsAny<Stream>()))
                    .ReturnsAsync(expected);
            var sut = CreateSUT();
            var fileName = "NAME" + extension;

            var fileUpload = CreateFileUpload("NAME" + extension);

            var result = await sut.ProcessFilesAsync([fileUpload]);
            Assert.That(result[0].Text, Is.EqualTo(expected));
            VerifyCalled(isOcrFallback: true);
        }


        [TestCase(".docx")]
        public async Task ProcessFilesAsync_Docx_Success(string extension)
        {
            const string expected = "DOCX";
            _fileParserMock = new Mock<IFileTextParser>();
            _fileParserMock.Setup(f => f.ReadDocxAsync(It.IsAny<Stream>()))
                .ReturnsAsync(expected);
            var sut = CreateSUT();
            var fileName = "NAME" + extension;

            var fileUpload = CreateFileUpload("NAME" + extension);

            var result = await sut.ProcessFilesAsync([fileUpload]);
            Assert.That(result[0].Text, Is.EqualTo(expected));
            VerifyCalled(isDocx: true);
        }


        [TestCase(".txt")]
        public async Task ProcessFilesAsync_TXT_Success(string extension)
        {
            const string expected = "TXT";
            _fileParserMock = new Mock<IFileTextParser>();
            _fileParserMock.Setup(f => f.ReadTextAsync(It.IsAny<Stream>()))
                .ReturnsAsync(expected);
            var sut = CreateSUT();
            var fileName = "NAME" + extension;

            var fileUpload = CreateFileUpload("NAME" + extension);

            var result = await sut.ProcessFilesAsync([fileUpload]);
            Assert.That(result[0].Text, Is.EqualTo(expected));
            VerifyCalled(isText: true);
        }


    }
}
