using Idams.Core.Enums;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Idams.Infrastructure.EntityFramework.Queries;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFDocumentManagementRepository : BaseRepository, IDocumentManagementRepository
    {
        public EFDocumentManagementRepository(IConfiguration configuration, ICurrentUserService currentService, IdamsDbContext dbContext) : base(configuration, currentService, dbContext)
        {
        }

        public async Task<Paged<DocumentManagementPaged>> GetPaged(TxDocumentFilter filter, int page = 1, int size = 10, string sort = "DateModified desc")
        {
            return await GetPaged<DocumentManagementPaged>(DocumentQuery.SelectPagedQuery, DocumentQuery.CountQuery, BuildDocumentManagementQuery(filter), page, size, sort);
        }


        public async Task<Paged<DocumentManagementPaged>> GetPagedByProject(TxDocumentFilter filter, int page, int size, string sort , string projectId)
        {
            return await GetPaged<DocumentManagementPaged>(DocumentQuery.SelectPagedQuery, DocumentQuery.CountQuery, BuildDocumentManagementQueryByProject(filter, projectId), page, size, sort);
        }



        private List<List<FilterModel>> BuildDocumentManagementQuery(TxDocumentFilter filter)
        {
            List<List<FilterModel>> ret = new List<List<FilterModel>>();
            BuildStageFilter(ret, filter);
            BuildRegionalFilter(ret, filter);
            BuildThresholdFilter(ret, filter);
            BuildWorkflowTypeFilter(ret, filter);
            BuildDocTypeFilter(ret, filter);
            BuildFileNameFilter(ret, filter);
            BuildDocCategoryFilter(ret, filter);
            BuildProjectNameFilter(ret, filter);
            BuildZonaFilter(ret, filter);
            BuildFileSizeFilter(ret, filter);
            BuildUploadByFilter(ret, filter);
            BuildHierLvl2Filter(ret, filter);
            BuildHierLvl3Filter(ret, filter);
            BuildFilterZonas(ret, filter);
            BuildRKAPFilter(ret, filter);
            BuildRevisionFilter(ret, filter);
            return ret;
        }

        private List<List<FilterModel>> BuildDocumentManagementQueryByProject(TxDocumentFilter filter, string projectId)
        {
            List<List<FilterModel>> ret = new List<List<FilterModel>>();
            BuildStageFilter(ret, filter);
            BuildWorkflowTypeFilter(ret, filter);
            BuildDocTypeFilter(ret, filter);
            BuildFileNameFilter(ret, filter);
            BuildDocCategoryFilter(ret, filter);
            BuildFileSizeFilter(ret, filter);
            BuildUploadByFilter(ret, filter);
            BuildFilterProjectId(ret, projectId);
            return ret;
        }
        private static void BuildFilterProjectId(List<List<FilterModel>> ret, string projectId)
        {
            ret.Add(new List<FilterModel> { new FilterModel("tph.ProjectId", FilterBuilderEnum.EQUALS, projectId) });
        }

        private static void BuildStageFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Stage.Count > 0)
            {
                List<FilterModel> stages = new List<FilterModel>();
                foreach(var data in filter.Stage)
                {
                    FilterModel model = new FilterModel("WorkflowCategoryParId", FilterBuilderEnum.EQUALS, data);
                    stages.Add(model);
                }
                ret.Add(stages);
            }
        }

        private static void BuildRegionalFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Regional.Count > 0)
            {
                List<FilterModel> regionals = new List<FilterModel>();
                foreach (var data in filter.Regional)
                {
                    FilterModel model = new FilterModel("HierLvl2", FilterBuilderEnum.EQUALS, data);
                    regionals.Add(model);
                }
                ret.Add(regionals);
            }
        }

        private static void BuildThresholdFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Threshold.Count > 0)
            {
                List<FilterModel> thresholds = new List<FilterModel>();
                foreach (var data in filter.Threshold)
                {
                    FilterModel model = new FilterModel("ThresholdNameParId", FilterBuilderEnum.EQUALS, data);
                    thresholds.Add(model);
                }
                ret.Add(thresholds);
            }
        }

        private static void BuildWorkflowTypeFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.WorkflowType.Count > 0)
            {
                List<FilterModel> workflowTypes = new List<FilterModel>();
                foreach (var data in filter.WorkflowType)
                {
                    FilterModel model = new FilterModel("WID", FilterBuilderEnum.EQUALS, data);
                    workflowTypes.Add(model);
                }
                ret.Add(workflowTypes);
            }
        }

        private static void BuildDocTypeFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.DocType.Count > 0)
            {
                List<FilterModel> docTypes = new List<FilterModel>();
                foreach (var data in filter.DocType)
                {
                    FilterModel model = new FilterModel("DocTypeParId", FilterBuilderEnum.EQUALS, data);
                    docTypes.Add(model);
                }
                ret.Add(docTypes);
            }
        }

        private static void BuildFileNameFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.FileName.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("DocName", FilterBuilderEnum.LIKE, filter.FileName[0]) });
            }
        }

        private static void BuildDocCategoryFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.DocCategory.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("DocCategory", FilterBuilderEnum.LIKE, filter.DocCategory[0]) });
            }
        }

        private static void BuildProjectNameFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.ProjectName.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("ProjectName", FilterBuilderEnum.LIKE, filter.ProjectName[0]) });
            }
        }

        private static void BuildZonaFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Zona.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3Desc", FilterBuilderEnum.LIKE, filter.Zona[0]) });
            }
        }

        private static void BuildFileSizeFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.FileSize.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("FileSize", FilterBuilderEnum.LIKE, filter.FileSize[0].ToString()) });
            }
        }

        private static void BuildUploadByFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.UploadBy.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("CreatedBy", FilterBuilderEnum.LIKE, filter.UploadBy[0]) });
            }
        }

        private static void BuildHierLvl2Filter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.HierLvl2))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl2", FilterBuilderEnum.EQUALS, filter.HierLvl2) });
            }
        }

        private static void BuildHierLvl3Filter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.HierLvl3))
            {
                ret.Add(new List<FilterModel> { new FilterModel("HierLvl3", FilterBuilderEnum.EQUALS, filter.HierLvl3) });
            }
        }

        private static void BuildFilterZonas(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Zonas.Count > 0)
            {
                List<FilterModel> zonas = new List<FilterModel>();
                foreach (var data in filter.Zonas)
                {
                    FilterModel model = new FilterModel("HierLvl3", FilterBuilderEnum.EQUALS, data);
                    zonas.Add(model);
                }
                ret.Add(zonas);
            }
        }

        private static void BuildRKAPFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.RKAP.Count > 0)
            {
                ret.Add(new List<FilterModel> { new FilterModel("cast(RKAP as varchar(15))", FilterBuilderEnum.LIKE, filter.RKAP[0]) });
            }
        }

        private static void BuildRevisionFilter(List<List<FilterModel>> ret, TxDocumentFilter filter)
        {
            if (filter.Revision.Count > 0)
            {
                List<FilterModel> revisions = new List<FilterModel>();
                foreach (var data in filter.Revision)
                {
                    FilterModel model = new FilterModel("Revision", FilterBuilderEnum.EQUALS, data.ToString());
                    revisions.Add(model);
                }
                ret.Add(revisions);
            }
        }
    }
}
