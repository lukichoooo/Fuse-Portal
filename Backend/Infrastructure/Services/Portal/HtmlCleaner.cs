using System.Text.RegularExpressions;
using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class HtmlCleaner : IHtmlCleaner
    {
        public string CleanHtml(string html)
        {
            html = Regex.Replace(html, "<script[^>]*?>[\\s\\S]*?</script>", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, "<style[^>]*?>[\\s\\S]*?</style>", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, "<!--.*?-->", "", RegexOptions.Singleline);

            html = Regex.Replace(html, "<img[^>]+>", "", RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"\s+", " ", RegexOptions.Multiline);

            return html.Trim();
        }
    }

}
