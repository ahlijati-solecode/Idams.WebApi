namespace Idams.Core.Model.Entities.Custom
{
    public class OutstandingTaskListPaged
    {
        public string ProjectId { get; set; }
        public string ProjectVersion { get; set; }
        public string ProjectName { get; set; }
        public string HierLvl2 { get; set; }
        public string HierLvl2Desc { get; set; }
        public string HierLvl3 { get; set; }
        public string HierLvl3Desc { get; set; }
        public string Threshold { get; set; }
        public string Stage { get; set; }
        public string WorkflowName { get; set; }
        public string WorkflowType { get; set; }
        public string WorkflowActionName { get; set; }
        public string WorkflowActionType { get; set; }
        public string DocumentGroupParId { get; set; }
        public string WorkflowActor { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

