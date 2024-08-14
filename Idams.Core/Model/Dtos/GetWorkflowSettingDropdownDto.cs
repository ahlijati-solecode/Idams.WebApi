namespace Idams.Core.Model.Dtos
{
    public class GetWorkflowSettingDropdownDto
    {
        public List<Dictionary<string, string>> ProjectCategory { get; set; }
        public List<Dictionary<string, string>> ProjectCriteria { get; set; }
        public List<Dictionary<string, string>> ProjectSubCriteria { get; set; }
        public List<Dictionary<string, string>> Threshold { get; set; }
    }

    public class GetScopeOfWorkDropdownDto
    {
        public List<Dictionary<string, string>> FieldService { get; set; }
        public List<Dictionary<string, string>> CompressorType { get; set; }
    }

    public class GetDocumentDropdownDto
    {
        public Dictionary<string, string> Documents { get; set; } = new Dictionary<string, string>();
    }
}
