namespace Idams.Core.Model.Requests
{
    public class UpdateScopeOfWorkRequest
    {
        public string ProjectId { get; set; } = null!;
        public int ProjectVersion { get; set; }
        public string? Section { get; set; }
        public bool? SaveLog { get; set; }
        public int? WellDrillProducerCount { get; set; }
        public int? WellDrillInjectorCount { get; set; }
        public int? WellWorkOverProducerCount { get; set; }
        public int? WellWorkOverInjectorCount { get; set; }

        public List<ProjectPlatformRequest> Platform { get; set; } = new List<ProjectPlatformRequest>();
        public List<ProjectCompressorRequest> Compressor { get; set; } = new List<ProjectCompressorRequest>();
        public List<ProjectPipelineRequest> Pipeline { get; set; } = new List<ProjectPipelineRequest>();
        public List<ProjectEquipmentRequest> Equipment { get; set; } = new List<ProjectEquipmentRequest>();
    }

    public class ProjectPlatformRequest
    {
        public int PlatformCount { get; set; }
        public int PlatformLegCount { get; set; }
    }

    public class ProjectCompressorRequest
    {
        public string? CompressorType { get; set; }
        public int CompressorCount { get; set; }
        public int CompressorCapacity { get; set; }
        public string? CompressorCapacityUoM { get; set; }
        public int CompressorDischargePressure { get; set; }
    }

    public class ProjectPipelineRequest
    {
        public string? FieldService { get; set; }
        public int PipelineCount { get; set; }
        public int PipelineLenght { get; set; }
    }

    public class ProjectEquipmentRequest
    {
        public string? EquipmentName { get; set; }
        public int EquipmentCount { get; set; }
    }
}
