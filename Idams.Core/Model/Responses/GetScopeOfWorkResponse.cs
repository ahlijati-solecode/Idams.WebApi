namespace Idams.Core.Model.Responses
{
    public class GetScopeOfWorkResponse
    {
        public int? WellDrillProducerCount { get; set; }
        public int? WellDrillInjectorCount { get; set; }
        public int? WellWorkOverProducerCount { get; set; }
        public int? WellWorkOverInjectorCount { get; set; }


        public List<ProjectPlatformResponse> Platform { get; set; } = new List<ProjectPlatformResponse>();
        public List<ProjectCompressorResponse> Compressor { get; set; } = new List<ProjectCompressorResponse>();
        public List<ProjectPipelineResponse> Pipeline { get; set; } = new List<ProjectPipelineResponse>();
        public List<ProjectEquipmentResponse> Equipment { get; set; } = new List<ProjectEquipmentResponse>();
    }

    public class ProjectPlatformResponse
    {
        public int PlatformCount { get; set; }
        public int PlatformLegCount { get; set; }
    }

    public class ProjectCompressorResponse
    {
        public string? CompressorType { get; set; }
        public int CompressorCount { get; set; }
        public int CompressorCapacity { get; set; }
        public string? CompressorCapacityUoM { get; set; }
        public int CompressorDischargePressure { get; set; }
    }

    public class ProjectPipelineResponse
    {
        public string? FieldService { get; set; }
        public int PipelineCount { get; set; }
        public int PipelineLenght { get; set; }
    }

    public class ProjectEquipmentResponse
    {
        public string? EquipmentName { get; set; }
        public int EquipmentCount { get; set; }
    }
}
