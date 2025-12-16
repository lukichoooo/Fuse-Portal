using Core.Interfaces.Portal;

namespace Infrastructure.Services.Portal
{
    public class ValidJsonExtractor : IValidJsonExtractor
    {
        public string ExtractJsonObject(string text)
        {
            int depth = 0;
            int start = -1;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '{')
                {
                    if (depth == 0)
                        start = i;
                    depth++;
                }
                else if (text[i] == '}')
                {
                    depth--;
                    if (depth == 0 && start != -1)
                        return text.Substring(start, i - start + 1);
                }
            }

            throw new InvalidOperationException("No valid JSON object found.");
        }
    }
}
