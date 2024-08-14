namespace Idams.Core.Model.Dtos
{
    public class MpDocumentChecklistDto
    {
        public string? GroupName { get; set; }
        public List<string> DocList { get; set; }


        public MpDocumentChecklistDto()
        {
            DocList = new List<string>();
        }
    }
}
