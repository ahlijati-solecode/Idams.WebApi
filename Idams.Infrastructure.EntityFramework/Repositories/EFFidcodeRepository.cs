using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Idams.Infrastructure.EntityFramework.Repositories
{
    public class EFFidcodeRepository : BaseRepository, IFidcodeRepository
    {
        IParameterListRepository _parameterListRepository;
        private readonly ILogger<EFFidcodeRepository> _logger;

        public EFFidcodeRepository(
            IConfiguration configuration,
            ICurrentUserService currentService,
            IdamsDbContext dbContext,
            IParameterListRepository parameterListRepository,
            ILogger<EFFidcodeRepository> logger) : base(configuration, currentService, dbContext)
        {
            _parameterListRepository = parameterListRepository;
            _logger = logger;
        }

        public async Task<string> Add(string subholdingCode, string projectCategory, int approvedYear, string regional)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                MdParamaterListDto? projectCategoryParam = await _parameterListRepository.GetParam("idams", "ProjectCategory", projectCategory);

                if (projectCategoryParam == null || projectCategoryParam.ParamValue2Text == null)
                {
                    throw new InvalidDataException("project category not found");
                }

                MdParamaterListDto? regionalParam = await _parameterListRepository.GetParam("idams", "FIDRegional", regional);

                if (regionalParam == null || regionalParam.ParamValue1 == null)
                {
                    throw new InvalidDataException("regional not found");
                }

                int regionalInt = Convert.ToInt32(regionalParam.ParamValue1);

                MdFidcode? existingFidcode = await _dbContext.MdFidcodes.AsNoTracking().Where(
                    e => e.SubholdingCode == subholdingCode &&
                    e.ProjectCategory == projectCategoryParam.ParamValue2Text &&
                    e.ApprovedYear == approvedYear &&
                    e.Regional == regionalInt).SingleOrDefaultAsync();

                if (existingFidcode == null)
                {
                    int min = 1000;
                    int max = 1999;

                    switch (regionalInt)
                    {
                        case 2:
                            min = 2000;
                            max = 2999;
                            break;
                        case 3:
                            min = 3000;
                            max = 3999;
                            break;
                        case 4:
                            min = 4000;
                            max = 4999;
                            break;
                        case 5:
                            min = 5000;
                            max = 5999;
                            break;
                        default:
                            break;
                    }

                    MdFidcode newFidcode = new MdFidcode
                    {
                        SubholdingCode = subholdingCode,
                        ProjectCategory = projectCategoryParam.ParamValue2Text,
                        ApprovedYear = approvedYear,
                        Regional = regionalInt,
                        Min = min,
                        Max = max,
                        LastNumber = min + 1
                    };

                    await _dbContext.MdFidcodes.AddAsync(newFidcode);

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return subholdingCode + projectCategoryParam.ParamValue2Text + approvedYear.ToString().Substring(2, 2) + newFidcode.LastNumber;
                }

                if (existingFidcode.LastNumber == existingFidcode.Max)
                {
                    throw new InvalidDataException("max sequence limit exceeded");
                }

                existingFidcode.LastNumber = existingFidcode.LastNumber + 1;

                _dbContext.MdFidcodes.Update(existingFidcode);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return subholdingCode + projectCategoryParam.ParamValue2Text + approvedYear.ToString().Substring(2, 2) + existingFidcode.LastNumber;
            }
            catch(Exception ex)
            {
                transaction.Rollback();

                _logger.LogError(ex, "Error add fidcode");

                throw;
            }
        }
    }
}

