using Idams.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IProjectUpstreamEntityRepository
    {
        Task<int> SaveProjectUpstreamEntity(string projectId, List<string> EntitiesId);
        Task<List<TxProjectUpstreamEntity>> GetProjectUpstreamEntities(string projectId);
    }
}
