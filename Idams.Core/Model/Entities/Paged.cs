namespace Idams.Core.Model.Entities
{
    public class Paged<TEntity>
    {
        public int TotalItems { get; set; }
        public IEnumerable<TEntity> Items { get; set; }
    }
}