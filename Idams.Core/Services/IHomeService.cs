namespace Idams.Core.Services
{
    public interface IHomeService
    {
        Task<Dictionary<string, string>> GetAboutIdamsUrls();
    }
}

