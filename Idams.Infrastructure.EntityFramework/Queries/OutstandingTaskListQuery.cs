namespace Idams.Infrastructure.EntityFramework.Queries
{
    public class OutstandingTaskListQuery
    {
		public const string SelectPagedQuery = @"
with projectList AS
(
select
	tph.ProjectId,
	tph.ProjectVersion,
	tph.ProjectName,
	vs2.HierLvl2,
	vs2.HierLvl2Desc,
	vs2.HierLvl3,
	vs2.HierLvl3Desc,
	mplTh.ParamListID as ThresholdParId,
	mplTh.ParamValue1Text as Threshold,
	mplStg.ParamValue1Text as Stage,
	rtws.WorkflowName,
	rw.WorkflowType,
	rwa.WorkflowActionName,
	rwa.WorkflowActionTypeParId as WorkflowActionType,
	rwa.RequiredDocumentGroupParId as DocumentGroupParId,
	tph.UpdatedDate,
	tph.Status,
	rwaa.Actor,
	tph.Deleted,
	tph.IsActive
from idams.TX_ProjectHeader tph 
inner join 
(
	select distinct tph2.ProjectId,
	stuff((
		select distinct ', ' + vs.HierLvl4Desc + ' (' + ISNULL(NULLIF(vs.CompanyCodeDesc,''),'-') + ')'
		from idams.TX_ProjectUpstreamEntity tpue
		join idams.vw_SHUHier03 vs on vs.EntityID = tpue.EntityId 
		where tpue.ProjectId = tph2.ProjectId 
		FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS HierLvl4
	from idams.TX_ProjectHeader tph2 
) as grouped on grouped.ProjectId = tph.ProjectId
join idams.TX_ProjectUpstreamEntity tpue on tpue.ProjectId = tph.ProjectId and tpue.EntityId = (select top 1 tpue2.EntityId from idams.TX_ProjectUpstreamEntity tpue2 where tpue2.ProjectId = tph.ProjectId)
join idams.vw_SHUHier03 vs2 on vs2.EntityID = tpue.EntityId 
join idams.REF_Template rt on rt.TemplateId = tph.TemplateId AND rt.TemplateVersion = tph.TemplateVersion
join dbo.MD_ParamaterList mplTh on mplTh.[Schema] = 'idams' and mplTh.ParamID = 'ThresholdName' and mplTh.ParamListID = rt.ThresholdNameParId
left join dbo.MD_ParamaterList mplSub on mplSub.[Schema] = 'idams' and mplSub.ParamID = 'SubProjectCriteria' and mplSub.ParamListID = rt.ProjectSubCriteriaParId
join idams.REF_TemplateWorkflowSequence rtws on rtws.WorkflowSequenceId = tph.CurrentWorkflowSequence
join idams.REF_Workflow rw on rtws.WorkflowId = rw.WorkflowId 
join idams.TX_ProjectAction tpa on tpa.ProjectActionId = tph.CurrentAction
join idams.REF_WorkflowAction rwa on tpa.WorkflowActionId = rwa.WorkflowActionId 
join dbo.MD_ParamaterList mplStg on mplStg.[Schema] = 'idams' and mplStg.ParamID = 'Stage' and mplStg.ParamListID = rw.WorkflowCategoryParId
cross apply (select rwaa2.Actor from idams.REF_WorkflowActor rwaa2 where rwaa2.WorkflowActionId = rwa.WorkflowActionId) rwaa
)
select * from
(
select
	p2.*,
	STUFF((SELECT ',' + CAST(p3.Actor AS varchar) FROM projectList p3 where p3.ProjectId = p2.ProjectId FOR XML PATH('')), 1, 1, '') AS WorkflowActor
from
(
select distinct
	p1.ProjectId,
	p1.ProjectVersion,
	p1.ProjectName,
	p1.HierLvl2,
	p1.HierLvl2Desc,
	p1.HierLvl3,
	p1.HierLvl3Desc,
	p1.ThresholdParId,
	p1.Threshold,
	p1.Stage,
	p1.WorkflowName,
	p1.WorkflowType,
	p1.WorkflowActionName,
	p1.WorkflowActionType,
	p1.DocumentGroupParId,
	p1.UpdatedDate,
	p1.Status,
	p1.Deleted,
	p1.IsActive
from projectList p1
) p2
) p4";

		public const string CountQuery = @"
with projectList AS
(
select
	tph.ProjectId,
	tph.ProjectVersion,
	tph.ProjectName,
	vs2.HierLvl2,
	vs2.HierLvl2Desc,
	vs2.HierLvl3,
	vs2.HierLvl3Desc,
	mplTh.ParamListID as ThresholdParId,
	mplTh.ParamValue1Text as Threshold,
	mplStg.ParamValue1Text as Stage,
	rtws.WorkflowName,
	rw.WorkflowType,
	rwa.WorkflowActionName,
	rwa.WorkflowActionTypeParId as WorkflowActionType,
	rwa.RequiredDocumentGroupParId as DocumentGroupParId,
	tph.UpdatedDate,
	tph.Status,
	rwaa.Actor,
	tph.Deleted,
	tph.IsActive
from idams.TX_ProjectHeader tph 
inner join 
(
	select distinct tph2.ProjectId,
	stuff((
		select distinct ', ' + vs.HierLvl4Desc + ' (' + ISNULL(NULLIF(vs.CompanyCodeDesc,''),'-') + ')'
		from idams.TX_ProjectUpstreamEntity tpue
		join idams.vw_SHUHier03 vs on vs.EntityID = tpue.EntityId 
		where tpue.ProjectId = tph2.ProjectId 
		FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS HierLvl4
	from idams.TX_ProjectHeader tph2 
) as grouped on grouped.ProjectId = tph.ProjectId
join idams.TX_ProjectUpstreamEntity tpue on tpue.ProjectId = tph.ProjectId and tpue.EntityId = (select top 1 tpue2.EntityId from idams.TX_ProjectUpstreamEntity tpue2 where tpue2.ProjectId = tph.ProjectId)
join idams.vw_SHUHier03 vs2 on vs2.EntityID = tpue.EntityId 
join idams.REF_Template rt on rt.TemplateId = tph.TemplateId AND rt.TemplateVersion = tph.TemplateVersion
join dbo.MD_ParamaterList mplTh on mplTh.[Schema] = 'idams' and mplTh.ParamID = 'ThresholdName' and mplTh.ParamListID = rt.ThresholdNameParId
left join dbo.MD_ParamaterList mplSub on mplSub.[Schema] = 'idams' and mplSub.ParamID = 'SubProjectCriteria' and mplSub.ParamListID = rt.ProjectSubCriteriaParId
join idams.REF_TemplateWorkflowSequence rtws on rtws.WorkflowSequenceId = tph.CurrentWorkflowSequence
join idams.REF_Workflow rw on rtws.WorkflowId = rw.WorkflowId 
join idams.TX_ProjectAction tpa on tpa.ProjectActionId = tph.CurrentAction
join idams.REF_WorkflowAction rwa on tpa.WorkflowActionId = rwa.WorkflowActionId 
join dbo.MD_ParamaterList mplStg on mplStg.[Schema] = 'idams' and mplStg.ParamID = 'Stage' and mplStg.ParamListID = rw.WorkflowCategoryParId
cross apply (select rwaa2.Actor from idams.REF_WorkflowActor rwaa2 where rwaa2.WorkflowActionId = rwa.WorkflowActionId) rwaa
)
select count(1) from 
(
	select
		p2.*,
		STUFF((SELECT ',' + CAST(p3.Actor AS varchar) FROM projectList p3 where p3.ProjectId = p2.ProjectId FOR XML PATH('')), 1, 1, '') AS WorkflowActor
	from
	(
	select distinct
		p1.ProjectId,
		p1.ProjectVersion,
		p1.ProjectName,
		p1.HierLvl2,
		p1.HierLvl2Desc,
		p1.HierLvl3,
		p1.HierLvl3Desc,
		p1.ThresholdParId,
		p1.Threshold,
		p1.Stage,
		p1.WorkflowName,
		p1.WorkflowType,
		p1.WorkflowActionName,
		p1.WorkflowActionType,
		p1.DocumentGroupParId,
		p1.UpdatedDate,
		p1.Status,
		p1.Deleted,
		p1.IsActive
	from projectList p1
	) p2
) p4";
	}
}

