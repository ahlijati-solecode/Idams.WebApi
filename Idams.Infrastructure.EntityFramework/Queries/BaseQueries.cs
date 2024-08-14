namespace Idams.Infrastructure.EntityFramework.Queries
{
    public class BaseQueries
    {
        public static string PagedQuery = string.Format(@"SELECT @select  @filter ORDER BY @sort offset @offset rows fetch next @limit rows only");
        public static string Query = string.Format(@"SELECT @select @filter");
        public const string BasePagedQuery = "@select @filter ORDER BY @sort offset @offset rows fetch next @limit rows only";
        public const string BaseCountQuery = "@select @filter";
    }
}
