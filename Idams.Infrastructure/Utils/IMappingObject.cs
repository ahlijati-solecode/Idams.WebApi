namespace Idams.Infrastructure.Utils
{
    public interface IMappingObject
    {
        TDestination Map<TDestination>(object model);
    }
}
