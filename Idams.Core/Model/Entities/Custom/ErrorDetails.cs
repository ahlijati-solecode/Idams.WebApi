using System.Text.Json;

namespace Idams.Core.Model.Entities.Custom
{
    public class ErrorDetails
    {
        public int? code { get; set; }
        public string? status { get; set; }
        public string? message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
