using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;

namespace Idams.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProjectRepository _projectRepository;

        public DashboardService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<DashboardDto> GetData(string? regionalFilter, string? rkapFilter, string? thresholdFilter)
        {
            DashboardDto data = new DashboardDto {
                Stage = new Dictionary<string, Dictionary<string, Dictionary<string, decimal>>>(),
                Total = new Dictionary<string, decimal>()
            };

            Paged<ProjectListPaged>? pagedProjectList = await _projectRepository.GetPaged(new ProjectListFilter(), 1, int.MaxValue, "HierLvl2Desc asc");

            IEnumerable<ProjectListPaged>? projects = pagedProjectList.Items;

            List<string> regionalFiltersList = string.IsNullOrEmpty(regionalFilter) ? new List<string>() : regionalFilter.Split(",").Select(e => e).ToList();
            List<string> thresholdFiltersList = string.IsNullOrEmpty(thresholdFilter) ? new List<string>() : thresholdFilter.Split(",").Select(e => e).ToList();
            List<string> rkapFilterList = string.IsNullOrEmpty(rkapFilter) ? new List<string>() : rkapFilter.Split(";").Select(e => e).ToList();

            if (regionalFiltersList.Count > 0)
            {
                projects = projects.Where(e => regionalFiltersList.Contains(e.HierLvl2Desc));
            }

            if (thresholdFiltersList.Count > 0)
            {
                projects = projects.Where(e => thresholdFiltersList.Contains(e.Threshold));
            }

            List<ProjectListPaged>? filteredProjects = new List<ProjectListPaged>();
            bool rkapFilterValid = false;

            if (rkapFilterList.Count > 0)
            {
                foreach (string r in rkapFilterList)
                {
                    try
                    {
                        List<string> rr = r.Split(",").Select(e => e).ToList();
                        int rkap = Convert.ToInt32(rr[0]);
                        bool revisi = rr.Count <= 1 ? false : rr[1] == "1";

                        filteredProjects.AddRange(projects.Where(e => e.RKAP == rkap && e.Revision == revisi));
                        rkapFilterValid = true;
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }

            if (!rkapFilterValid)
            {
                filteredProjects.AddRange(projects);
            }

            List<ProjectListPaged>? inisiasiData = filteredProjects.Where(e => e.Stage == "Inisiasi" && e.Status == "In-Progress").ToList();
            List<ProjectListPaged>? seleksiData = filteredProjects.Where(e => e.Stage == "Seleksi" && e.Status == "In-Progress").ToList();
            List<ProjectListPaged>? kajianLanjutData = filteredProjects.Where(e => e.Stage == "Kajian Lanjut" && e.Status == "In-Progress").ToList();
            List<ProjectListPaged>? approvedData = filteredProjects.Where(e => e.Status == "Completed").ToList();

            data.Stage["Inisiasi"] = GetStageData(inisiasiData);
            data.Stage["Seleksi"] = GetStageData(seleksiData);
            data.Stage["Kajian Lanjut"] = GetStageData(kajianLanjutData);
            data.Stage["Completed"] = GetStageData(approvedData);

            data.Total["Inisiasi"] = inisiasiData.Count;
            data.Total["Seleksi"] = seleksiData.Count;
            data.Total["Kajian Lanjut"] = kajianLanjutData.Count;
            data.Total["Completed"] = approvedData.Count;

            data.RkapDropdown = projects.Select(e => e.RKAP.ToString()).Distinct().OrderBy(e => e).ToList();

            return data;
        }

        private Dictionary<string, Dictionary<string, decimal>> GetStageData(List<ProjectListPaged>? data)
        {
            Dictionary<string, Dictionary<string, decimal>> stageData = new Dictionary<string, Dictionary<string, decimal>>();

            stageData["Project"] = new Dictionary<string, decimal>();
            stageData["Capex"] = new Dictionary<string, decimal>();
            stageData["Oil"] = new Dictionary<string, decimal>();
            stageData["Gas"] = new Dictionary<string, decimal>();
            stageData["Resources"] = new Dictionary<string, decimal>();
            stageData["Wells"] = new Dictionary<string, decimal>();
            stageData["Workover"] = new Dictionary<string, decimal>();

            if (data == null)
            {
                return stageData;
            }

            foreach (ProjectListPaged p in data)
            {
                if (!stageData["Project"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Project"][p.HierLvl2Desc] = 1;
                }
                else
                {
                    stageData["Project"][p.HierLvl2Desc] += 1;
                }

                if (!stageData["Capex"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Capex"][p.HierLvl2Desc] = (decimal)p.Capex;
                }
                else
                {
                    stageData["Capex"][p.HierLvl2Desc] += (decimal)p.Capex;
                }

                if (!stageData["Oil"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Oil"][p.HierLvl2Desc] = (decimal)p.Oil;
                }
                else
                {
                    stageData["Oil"][p.HierLvl2Desc] += (decimal)p.Oil;
                }

                if (!stageData["Gas"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Gas"][p.HierLvl2Desc] = (decimal)p.Gas;
                }
                else
                {
                    stageData["Gas"][p.HierLvl2Desc] += (decimal)p.Gas;
                }

                if (!stageData["Resources"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Resources"][p.HierLvl2Desc] = (decimal)p.OilEquivalent;
                }
                else
                {
                    stageData["Resources"][p.HierLvl2Desc] += (decimal)p.OilEquivalent;
                }

                if (!stageData["Wells"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Wells"][p.HierLvl2Desc] = (p.WellDrillProducerCount ?? 0) + (p.WellDrillInjectorCount ?? 0);
                }
                else
                {
                    stageData["Wells"][p.HierLvl2Desc] += (p.WellDrillProducerCount ?? 0) + (p.WellDrillInjectorCount ?? 0);
                }

                if (!stageData["Workover"].ContainsKey(p.HierLvl2Desc))
                {
                    stageData["Workover"][p.HierLvl2Desc] = (p.WellWorkOverProducerCount ?? 0) + (p.WellWorkOverInjectorCount ?? 0);
                }
                else
                {
                    stageData["Workover"][p.HierLvl2Desc] += (p.WellWorkOverProducerCount ?? 0) + (p.WellWorkOverInjectorCount ?? 0);
                }
            }

            return stageData;
        }
    }
}

