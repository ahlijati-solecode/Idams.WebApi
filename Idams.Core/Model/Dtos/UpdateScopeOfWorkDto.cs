using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Model.Dtos
{
    public class UpdateScopeOfWorkDto
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string Section { get; set; }
        public bool? SaveLog { get; set; }
        public int? WellDrillProducerCount { get; set; }
        public int? WellDrillInjectorCount { get; set; }
        public int? WellWorkOverProducerCount { get; set; }
        public int? WellWorkOverInjectorCount { get; set; }

        public List<ProjectPlatformDto> Platform { get; set; } = new List<ProjectPlatformDto>();
        public List<ProjectCompressorDto> Compressor { get; set; } = new List<ProjectCompressorDto>();
        public List<ProjectPipelineDto> Pipeline { get; set; } = new List<ProjectPipelineDto>();
        public List<ProjectEquipmentDto> Equipment { get; set; } = new List<ProjectEquipmentDto>();
    }

    public class ProjectPlatformDto
    {
        public int PlatformCount { get; set; }
        public int PlatformLegCount { get; set; }
    }

    public class ProjectCompressorDto
    {
        public string? CompressorType { get; set; }
        public int CompressorCount { get; set; }
        public int CompressorCapacity { get; set; }
        public string? CompressorCapacityUoM { get; set; }
        public int CompressorDischargePressure { get; set; }
    }

    public class ProjectPipelineDto
    {
        public string? FieldService { get; set; }
        public int PipelineCount { get; set; }
        public int PipelineLenght { get; set; }
    }

    public class ProjectEquipmentDto
    {
        public string? EquipmentName { get; set; }
        public int EquipmentCount { get; set; }
    }

    public class GetScopeOfWorkDto : UpdateScopeOfWorkDto
    {
    }
}
