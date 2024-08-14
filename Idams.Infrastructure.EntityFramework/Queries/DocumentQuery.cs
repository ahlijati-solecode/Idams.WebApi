namespace Idams.Infrastructure.EntityFramework.Queries
{
    public class DocumentQuery
    {
        public static string SelectPagedQuery = @"
			select td.TransactionDocId,
				   td.DocName as FileName,
				   mdd.DocType,
				   mdd.DocCategory,
				   tph.ProjectName,
				   upsHier.HierLvl2Desc as ""Regional"",
				   upsHier.HierLvl3Desc as ""Zona"",
				   td.Threshold,
				   rw.Stage,
				   tph.RKAP,
				   tph.Revision,
				   rw.WID,
				   rw.WorkflowType,
				   td.FileSize,
				   td.FileExtension,
				   td.CreatedBy as ""UploadBy"",
				   td.CreatedDate as ""DateModified"" 
			from(
			  select DocDescriptionId,
			  TransactionDocId,
			  DocName,
			  ThresholdNameParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'ThresholdName' and ParamListID = ThresholdNameParId) as Threshold,
			  ProjectActionId,
			  WorkflowActionId,
			  FileSize,
			  CreatedBy,
			  CreatedDate,
			  FileExtension
			  from idams.TX_Document where IsActive = 1
			)td
			left join(
			  select DocDescriptionId,
			   DocTypeParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'DocType' and ParamListID = DocTypeParId) as DocType,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema""= 'Idams' and ParamID = 'Stage' and ParamListID = DocCategoryParId) as DocCategory
			  from idams.MD_DocumentDescription 
			)mdd
			on td.DocDescriptionId = mdd.DocDescriptionId
			left join(
			  select projectActionId, ProjectId from idams.TX_ProjectAction
			)tpa
			on td.ProjectActionId = tpa.ProjectActionId
			left join(
			  select ProjectId, ProjectName, RKAP, Revision
			  from idams.TX_ProjectHeader where IsActive = 1
			)tph
			on tpa.ProjectId = tph.ProjectId
			left join(
			   select distinct ProjectId, HierLvl2, HierLvl2Desc, HierLvl3, HierLvl3Desc
			   from idams.TX_ProjectUpstreamEntity ups
			   inner join(
				  select* from idams.vw_SHUHier03
			   )hier
			   on ups.EntityId = hier.EntityID
			)upsHier
			on tph.ProjectId = upsHier.ProjectId
			left join(
			  select WorkflowActionId, WorkflowId from idams.REF_WorkflowAction
			)rwa
			on td.WorkflowActionId  = rwa.WorkflowActionId
			left join(
			  select WorkflowId as WID, WorkflowType,WorkflowCategoryParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'Stage' and ParamListID = WorkflowCategoryParId) as Stage
			  from idams.REF_Workflow 
			)rw
			on rwa.WorkflowId  = rw.WID";


		public static string CountQuery = @"
			select count(1)
			from(
			  select DocDescriptionId,
			  TransactionDocId,
			  DocName,
			  ThresholdNameParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'ThresholdName' and ParamListID = ThresholdNameParId) as Threshold,
			  ProjectActionId,
			  WorkflowActionId,
			  FileSize,
			  CreatedBy,
			  CreatedDate,
			  FileExtension
			  from idams.TX_Document where IsActive = 1
			)td
			left join(
			  select DocDescriptionId,
			  DocTypeParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'DocType' and ParamListID = DocTypeParId) as DocType,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema""= 'Idams' and ParamID = 'Stage' and ParamListID = DocCategoryParId) as DocCategory
			  from idams.MD_DocumentDescription 
			)mdd
			on td.DocDescriptionId = mdd.DocDescriptionId
			left join(
			  select projectActionId, ProjectId from idams.TX_ProjectAction
			)tpa
			on td.ProjectActionId = tpa.ProjectActionId
			left join(
			  select ProjectId, ProjectName, RKAP, Revision
			  from idams.TX_ProjectHeader where IsActive = 1
			)tph
			on tpa.ProjectId = tph.ProjectId
			left join(
			   select distinct ProjectId, HierLvl2, HierLvl2Desc, HierLvl3, HierLvl3Desc
			   from idams.TX_ProjectUpstreamEntity ups
			   inner join(
				  select* from idams.vw_SHUHier03
			   )hier
			   on ups.EntityId = hier.EntityID
			)upsHier
			on tph.ProjectId = upsHier.ProjectId
			left join(
			  select WorkflowActionId, WorkflowId from idams.REF_WorkflowAction
			)rwa
			on td.WorkflowActionId  = rwa.WorkflowActionId
			left join(
			  select WorkflowId as WID, WorkflowType, WorkflowCategoryParId,
			  (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'Idams' and ParamID = 'Stage' and ParamListID = WorkflowCategoryParId) as Stage
			  from idams.REF_Workflow 
			)rw
			on rwa.WorkflowId  = rw.WID";
	}
}
