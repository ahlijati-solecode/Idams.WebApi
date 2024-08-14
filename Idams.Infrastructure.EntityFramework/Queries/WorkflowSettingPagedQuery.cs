namespace Idams.Infrastructure.EntityFramework.Queries
{
	public class WorkflowSettingPagedQuery
	{
		public static string SelectPagedQuery = @" 
				rt.TemplateId,
				rt.TemplateVersion,
				rt.TemplateName, 
				rt.ProjectCategory,
				rt.ProjectCriteria,
				rt.ProjectSubCriteria,
				rtp.Threshold,
				rtp.CapexValue,
				rt.TotalWorkflow,
				rt.StartWorkflow,
				rt.EndWorkflow,
				rt.Status as ""Status"",
				rt.CreatedDate,
                rt.UpdatedDate
			from
			(select TemplateName, TemplateId, TemplateVersion,TotalWorkflow, StartWorkflow, EndWorkflow, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, Status,
			 (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'ProjectCategory' and ParamListID  = ProjectCategoryParId) as ProjectCategory,
			 (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'ProjectCriteria' and ParamListID = ProjectCriteriaParId) as ProjectCriteria,
			 (select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'SubProjectCriteria' and ParamListID = ProjectSubCriteriaParId) as ProjectSubCriteria,
			 ThresholdNameParId ,
			 ThresholdVersion
			 from idams.REF_Template where Deleted = 0
			 )rt
			left join
			(select ThresholdNameParId, ThresholdVersion ,
			 (select ParamValue1Text from MD_ParamaterList where ParamListID = ThresholdNameParId) as Threshold,
			 (case
  				when ThresholdNameParId = 'SHU' then concat('$', convert(varchar(100), cast(Capex1 as money), 1), ' ', (select left(ParamValue1Text, 1)  from MD_ParamaterList where ParamListID = MathOperatorParId), ' $', convert(varchar(100), cast(Capex2 as money), 1))
  				else concat((select left(ParamValue1Text, 1)  from MD_ParamaterList where ParamListID = MathOperatorParId), ' $', convert(varchar(100), cast(Capex1 as money), 1))
			  end) as CapexValue
			  from idams.REF_ThresholdProject
			 )rtp
			 on rt.ThresholdNameParId  = rtp.ThresholdNameParId
			 and rt.ThresholdVersion = rtp.ThresholdVersion
			";


		public static string CountQuery = @"
				count(1)
			from
			(select TemplateName, TemplateId, TemplateVersion,TotalWorkflow, StartWorkflow, EndWorkflow, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, IsActive, Status,
				(select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'ProjectCategory' and ParamListID  = ProjectCategoryParId) as ProjectCategory,
				(select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'ProjectCriteria' and ParamListID = ProjectCriteriaParId) as ProjectCriteria,
				(select ParamValue1Text from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'SubProjectCriteria' and ParamListID = ProjectSubCriteriaParId) as ProjectSubCriteria,
				ThresholdNameParId ,
				ThresholdVersion
				from idams.REF_Template where Deleted = 0
				)rt
			left join
			(select ThresholdNameParId, ThresholdVersion ,
				(select ParamValue1Text from MD_ParamaterList where ParamListID = ThresholdNameParId) as Threshold,
				(case
  				when ThresholdNameParId = 'SHU' then concat('$', convert(varchar(100), cast(Capex1 as money), 1), ' ', (select left(ParamValue1Text, 1)  from MD_ParamaterList where ParamListID = MathOperatorParId), ' $', convert(varchar(100), cast(Capex2 as money), 1))
  				else concat((select left(ParamValue1Text, 1)  from MD_ParamaterList where ParamListID = MathOperatorParId), ' $', convert(varchar(100), cast(Capex1 as money), 1))
				end) as CapexValue
				from idams.REF_ThresholdProject
				)rtp
				on rt.ThresholdNameParId  = rtp.ThresholdNameParId
				and rt.ThresholdVersion = rtp.ThresholdVersion
			";
	}

}
