namespace Idams.Core.Model.Responses
{
    public class GetWorkflowSettingDropdownResponse
    {
        public List<Dictionary<string, string>> ProjectCategory { get; set; }
        public List<Dictionary<string, string>> ProjectCriteria { get; set; }
        public List<Dictionary<string, string>> ProjectSubCriteria { get; set; }
        public List<Dictionary<string, string>> Threshold { get; set; }
    }

    public class GetScopeOfWorkDropdownResponse
    {
        public List<Dictionary<string, string>>  FieldService{ get; set; }
        public List<Dictionary<string, string>> CompressorType { get; set; }
    }

    public class GetDocumentDropdownResponse
    {
        public Dictionary<string, string> Documents { get; set; } = new Dictionary<string, string>();
    }

}
