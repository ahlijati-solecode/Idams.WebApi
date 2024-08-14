using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Queries
{
    public class ProjectQuery
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
	grouped.HierLvl4,
	mplTh.ParamListID as ThresholdParId,
	mplTh.ParamValue1Text as Threshold,
	mplSub.ParamValue1Text as SubCriteria,
	tph.DrillingCost,
	tph.FacilitiesCost,
	tph.Capex,
	tph.EstFIDApproved,
	tph.RKAP,
	tph.Revision,
	tph.WellDrillProducerCount,
	tph.WellDrillInjectorCount,
	tph.WellWorkOverProducerCount,
	tph.WellWorkOverInjectorCount,
	mplStg.ParamValue1Text as Stage,
	rtws.WorkflowName,
	rw.WorkflowType,
	tph.ProjectOnStream,
	tph.NetPresentValue,
	tph.InternalRateOfReturn,
	tph.ProfitabilityIndex,
	tph.Oil,
	tph.Gas,
	tph.OilEquivalent,
	tph.PVIn,
	tph.PVOut,
	tph.BenefitCostRatio,
	tph.InitiationDate,
	tph.EndDate,
	rwa.WorkflowActionName,
	tph.Status,
	tph.UpdatedDate,
	tph.Deleted,
	tph.Fidcode,
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
)
select * from projectList";

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
	grouped.HierLvl4,
	mplTh.ParamListID as ThresholdParId,
	mplTh.ParamValue1Text as Threshold,
	mplSub.ParamValue1Text as SubCriteria,
	tph.DrillingCost,
	tph.FacilitiesCost,
	tph.Capex,
	tph.EstFIDApproved,
	tph.RKAP,
	tph.Revision,
	mplStg.ParamValue1Text as Stage,
	rtws.WorkflowName,
	rw.WorkflowType,
	tph.ProjectOnStream,
	tph.NetPresentValue,
	tph.InternalRateOfReturn,
	tph.ProfitabilityIndex,
	tph.Oil,
	tph.Gas,
	tph.OilEquivalent,
	tph.InitiationDate ,
	tph.EndDate,
	rwa.WorkflowActionName,
	tph.Status,
	tph.UpdatedDate,
	tph.Deleted,
	tph.Fidcode,
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
)
select count(1) from projectList";

		public const string ProjectBannerData = @"
with pl as
(
select tph.ProjectId,
	mplStg.ParamListID,
	tph.Status,
	vs2.HierLvl2,
	vs2.hierlvl3,
	tph.Deleted,
	tph.IsActive
from idams.TX_ProjectHeader tph
join idams.TX_ProjectUpstreamEntity tpue on tpue.ProjectId = tph.ProjectId and tpue.EntityId = (select top 1 tpue2.EntityId from idams.TX_ProjectUpstreamEntity tpue2 where tpue2.ProjectId = tph.ProjectId)
join idams.vw_SHUHier03 vs2 on vs2.EntityID = tpue.EntityId 
join idams.REF_TemplateWorkflowSequence rtws on tph.CurrentWorkflowSequence = rtws.WorkflowSequenceId 
join idams.REF_Workflow rw on rtws.WorkflowId = rw.WorkflowId 
join dbo.MD_ParamaterList mplStg on mplStg.[Schema] = 'idams' and mplStg.ParamID = 'Stage' and mplStg.ParamListID = rw.WorkflowCategoryParId 
@filter
)
select
	(select count(1) from pl) as AllProject,
	(select count(1) from pl where pl.ParamListID = 'Inisiasi') as InisiasiProject,
	(select count(1) from pl where pl.ParamListID = 'Seleksi') as SeleksiProject,
	(select count(1) from pl where pl.ParamListID = 'KLanjut' and pl.Status != 'Completed') as KLanjutProject,
	(select count(1) from pl where pl.Status = 'Completed') as FIDApproved";

	}
}
