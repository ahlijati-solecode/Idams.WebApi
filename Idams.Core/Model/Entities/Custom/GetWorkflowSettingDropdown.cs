namespace Idams.Core.Model.Entities.Custom
{
    public class GetWorkflowSettingDropdown
    {
        public List<Dictionary<string, string>> ProjectCategory { get; set; }
        public List<Dictionary<string, string>> ProjectCriteria { get; set; }
        public List<Dictionary<string, string>> ProjectSubCriteria { get; set; }
        public List<Dictionary<string, string>> Threshold { get; set; }


        public GetWorkflowSettingDropdown()
        {
            ProjectCriteria = new List<Dictionary<string, string>>();
            ProjectCategory = new List<Dictionary<string, string>>();
            Threshold = new List<Dictionary<string, string>>();
            ProjectSubCriteria = new List<Dictionary<string, string>>();
        }
    }

    public class GetScopeOfWorkDropdown
    {
        public List<Dictionary<string, string>> FieldService { get; set; }
        public List<Dictionary<string, string>> CompressorType { get; set; }

        public GetScopeOfWorkDropdown()
        {
            FieldService = new List<Dictionary<string, string>>();
            CompressorType = new List<Dictionary<string, string>>();
        }
    }

}
