namespace Idams.Core.Repositories
{
    public interface IFidcodeRepository
    {
        Task<string> Add(string subholdingCode, string projectCategory, int approvedYear, string regional);
    }
}

