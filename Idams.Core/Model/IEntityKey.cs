namespace Idams.Core.Model
{
    public interface IEntityKey<TKey>
    {
        TKey Id { get; }
    }
}