namespace Idams.Core.Repositories
{
    public interface ITableColumnSettingRepository
    {
        Task<string> GetProjectListTableConfig(string userId);
        Task<int> SaveProjectListTableConfig(string userId, string config);
    }
}
