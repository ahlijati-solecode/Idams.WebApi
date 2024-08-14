using AutoMapper;
using Idams.Infrastructure.Utils;

namespace Idams.WebApi.Utils
{
    public class MappingObject : IMappingObject
    {
        private readonly IMapper _mapper;

        public MappingObject(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TDestination Map<TDestination>(object model)
        {
            return _mapper.Map<TDestination>(model);
        }
    }
}