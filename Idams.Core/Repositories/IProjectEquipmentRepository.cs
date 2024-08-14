using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IProjectEquipmentRepository
    {
        Task<List<TxProjectEquipment>> UpdateAsync(string projectId, int projectVersion, List<TxProjectEquipment> equipment);
        Task<List<TxProjectEquipment>> GetEquipmentsAsync(string projectId, int projectVersion);
        Task<List<TxProjectEquipment>> AddEquipmentAsync(List<TxProjectEquipment> equipments);
    }
}
