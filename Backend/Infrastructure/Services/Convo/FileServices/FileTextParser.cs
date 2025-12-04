using System.Text;
using Core.Interfaces.Convo.FileServices;
using NPOI.XWPF.UserModel;

namespace Infrastructure.Services.Convo.FileServices
{
    public class FileTextParser : IFileTextParser
    {
        public Task<string> ReadDocxAsync(Stream stream)
        {
            var doc = new XWPFDocument(stream);
            var sb = new StringBuilder();

            foreach (var para in doc.Paragraphs)
                sb.AppendLine(para.ParagraphText);

            foreach (var table in doc.Tables)
                foreach (var row in table.Rows)
                    foreach (var cell in row.GetTableCells())
                        sb.AppendLine(cell.GetText());

            return Task.FromResult(sb.ToString());
        }

        public async Task<string> ReadTextAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
