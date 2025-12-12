namespace Core.Interfaces.Portal
{
    public interface IMockExamCreator
    {
        public Task<string> GenerateExamAsync(string syllabi);
    }
}
