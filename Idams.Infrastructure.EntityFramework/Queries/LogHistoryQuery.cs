using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Queries
{
    public class LogHistoryQuery
    {
        public static string SelectPagedQuery = @"lpat.projectId,
												  lpat.EmpName,
												  rtws.WorkflowName,
												  lpat.[Action],
												  lpat.ActivityDescription,
												  rwa.WorkflowActionTypeParId as WorkflowActionType,
												  rwa.RequiredDocumentGroupParId as DocumentGroupParId,
												  mpl.ParamValue1Text as LastStatus,
												  lpat.CreatedDate as DateModified
			from
			(select * from idams.LG_ProjectActivityAuditTrail )lpat
			left join
			(select * from idams.REF_TemplateWorkflowSequence)rtws
			on lpat.WorkflowSequenceId = rtws.WorkflowSequenceId
			left join
			(select * from idams.REF_WorkflowAction) rwa
			on lpat.WorkflowActionId = rwa.WorkflowActionId
			left join
			(select * from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'Status')mpl
			on lpat.ActivityStatusParId = mpl.ParamListID";

		public static string CountQuery = @"count(1)	   
			from
			(select * from idams.LG_ProjectActivityAuditTrail )lpat
			left join
			(select * from idams.REF_TemplateWorkflowSequence)rtws
			on lpat.WorkflowSequenceId = rtws.WorkflowSequenceId
			left join
			(select * from idams.REF_WorkflowAction) rwa
			on lpat.WorkflowActionId = rwa.WorkflowActionId
			left join
			(select * from MD_ParamaterList where ""Schema"" = 'idams' and ParamID = 'ActivityStatus')mpl
			on lpat.ActivityStatusParId = mpl.ParamListID";
	}
}
