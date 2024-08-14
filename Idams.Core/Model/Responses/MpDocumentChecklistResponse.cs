namespace Idams.Core.Model.Responses
{
    public class MpDocumentChecklistResponse
    {
        public string? GroupName { get; set; }
        public List<string> DocList { get; set; }

        public MpDocumentChecklistResponse(List<string> docList)
        {
            DocList = docList;
        }
    }
}
