namespace Idams.Core.Model.Filters
{
    public class OutstandingTaskListFilter
    {
        public string? ProjectName { get; set; }
        public string? HierLvl2Desc { get; set; }
        public string? HierLvl3Desc { get; set; }
        public string? Threshold { get; set; }
        public string? Stage { get; set; }
        public string? WorkflowName { get; set; }
        public string? WorkflowActionName { get; set; }
        public string? WorkflowActionType { get; set; }
        public string? DocumentGroupParId { get; set; }
        public string? UpdatedDate { get; set; }

        #region dropdown filter
        public string? HierLvl2DropDown { get; set; }
        public string? ThresholdDropDown { get; set; }
        public string? StageDropdown { get; set; }
        public string? WorkflowType { get; set; }
        #endregion

        #region user privilege filter
        public string? HierLvl2 { get; set; }
        public string? HierLvl3 { get; set; }
        #endregion
    }
}

