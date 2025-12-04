using Core.Dtos;

namespace Core.Interfaces.Convo.FileServices
{
    public interface IFileProcessingService
    {
        Task<List<FileDto>> ProcessFilesAsync(List<FileUpload> files);
    }
}
