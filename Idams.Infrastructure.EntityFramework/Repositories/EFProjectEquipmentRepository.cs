
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using shortid;
using shortid.Configuration;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFProjectEquipmentRepository : BaseRepository, IProjectEquipmentRepository
    {
        public EFProjectEquipmentRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<List<TxProjectEquipment>> UpdateAsync(string projectId, int projectVersion, List<TxProjectEquipment> equipment)
        {
            var items = await _dbContext.TxProjectEquipments.Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();

            _dbContext.TxProjectEquipments.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
            List<TxProjectEquipment> txProjectEquipments = new List<TxProjectEquipment>();
            equipment.ForEach(n =>
            {
                TxProjectEquipment data = new TxProjectEquipment
                {
                    ProjectEquipmentId = ShortId.Generate(new GenerationOptions(true, false, 20)),
                    ProjectId = projectId,
                    ProjectVersion = projectVersion,
                    EquipmentCount = n.EquipmentCount,
                    EquipmentName = n.EquipmentName,
                };
                txProjectEquipments.Add(data);
            });
            await _dbContext.AddRangeAsync(txProjectEquipments);
            await _dbContext.SaveChangesAsync();
            return txProjectEquipments;
        }

        public async Task<List<TxProjectEquipment>> GetEquipmentsAsync(string projectId, int projectVersion)
        {
            return await _dbContext.TxProjectEquipments.AsNoTracking().Where(n => n.ProjectId == projectId && n.ProjectVersion == projectVersion).ToListAsync();
        }

        public async Task<List<TxProjectEquipment>> AddEquipmentAsync(List<TxProjectEquipment> equipments)
        {
            await _dbContext.AddRangeAsync(equipments);
            await _dbContext.SaveChangesAsync();
            return equipments;
        }

    }
}
