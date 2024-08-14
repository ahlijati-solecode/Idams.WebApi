using Idams.Core.Enums;
using Idams.Core.Http;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Idams.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IParameterListRepository _parameterListRepository;
        private readonly IHierLvlVwRepository _hierLvlVwRepository;
        private readonly IMappingObject _mapper;
        private readonly ILogger _logger;
        public string MadamToken { get; private set; }
        public string MadamUrl { get; private set; }
        public string PropUserData { get; private set; }
        public string PropUserMenu { get; private set; }
        public string PropEmployee { get; private set; }

        public UserService(IUserRepository userRepository,
            IParameterListRepository parameterListRepository,
            IHierLvlVwRepository hierLvlVwRepository,
            IMappingObject mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _parameterListRepository = parameterListRepository;
            _hierLvlVwRepository = hierLvlVwRepository;
            _mapper = mapper;
            _logger = logger;
            this.MadamToken = "";
            this.MadamUrl = "";
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            return _mapper.Map<UserDto>(await _userRepository.GetByIdAsync(id));
        }

        public async Task<UserDto> GetUser(string username)
        {
            try
            {
                await GetBaseUrlListAsync();
                var listParam = new List<ParamFilter>()
                {
                    new ParamFilter("idams", "Url", "GetUserData"),
                    new ParamFilter("idams", "Url", "GetAppMenu"),
                    new ParamFilter("idams", "Url", "GetAllMasterEmployee"),
                    new ParamFilter("idams", "AppFK", "AppFK"),
                };
                var paramList = await _parameterListRepository.GetParams(listParam);
                var UserDataUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "GetUserData")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
                var AppMenuUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "GetAppMenu")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
                var MasterEmployeeUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "GetAllMasterEmployee")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
                var appFk = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "AppFK")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";

                var userDataApp = await GetUserDataApplication(username, UserDataUrl, appFk);
                if (userDataApp != null)
                {
                    var userRoles = await GetUserRoles(userDataApp.Roles);
                    var userMenus = await GetUserMenu(userDataApp.AuthUserApp, AppMenuUrl);
                    var employeeData = await GetEmployee(username, MasterEmployeeUrl);
                    if (employeeData != null)
                    {
                        var hierLvl = await _hierLvlVwRepository.GetData(employeeData.OrgUnitID?.ToString());
                        return new UserDto()
                        {
                            Name = employeeData.EmpName,
                            EmpAccount = employeeData.EmpAccount,
                            Email = employeeData.EmpEmail,
                            EmpId = employeeData.EmpID,
                            Roles = userRoles,
                            Menu = userMenus,
                            HierLvl2 = new HierLvlDto { Key = hierLvl?.Lvl2EntityId!, Value = hierLvl?.Lvl2EntityName! },
                            HierLvl3 = new HierLvlDto { Key = hierLvl?.Lvl3EntityId!, Value = hierLvl?.Lvl3EntityName! }
                        };
                    }
                    else
                    {
                        throw new Exception("Employee Data Not Found");
                    }
                }
                else
                {
                    throw new Exception("User Data Not Found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<List<UserRoleDto>> GetUserRoles(List<UserRoleDto> rawRoles)
        {
            List<UserRoleDto> result = new();
            if (rawRoles == null)
                return result;
            
            foreach (var rawRole in rawRoles)
            {
                var param = await _parameterListRepository.GetParam("idams", "Roles", rawRole.Value);
                _ = Enum.TryParse(rawRole.Value, out RoleEnum role);
                result.Add(new UserRoleDto()
                {
                    Key = rawRole.Value,
                    Value = param?.ParamValue1Text!,
                    Enum = role
                });
            }
            return result;
        }

        public async Task<List<MenuDto>> GetUserMenu(string username)
        {
            try
            {
                await GetBaseUrlListAsync();
                var listParam = new List<ParamFilter>()
                {
                    new ParamFilter("idams", "Url", "GetUserData"),
                    new ParamFilter("idams", "Url", "GetAppMenu"),
                    new ParamFilter("idams", "AppFK", "AppFK"),
                };
                var paramList = await _parameterListRepository.GetParams(listParam);
                var UserDataUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "GetUserData")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
                var AppMenuUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "GetAppMenu")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
                var appFk = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "AppFK")
                    .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";

                var userDataApp = await GetUserDataApplication(username, UserDataUrl, appFk);
                if (userDataApp != null)
                {
                    var userMenus = await GetUserMenu(userDataApp.AuthUserApp, AppMenuUrl);
                    return userMenus;
                }
                else
                {
                    throw new Exception("User Data Not Found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<UserDataDto?> GetUserDataApplication(string username, string userDataUrl, string appFk)
        {

            UriBuilder uriBuilder = new UriBuilder(this.MadamUrl);
            uriBuilder.Path = userDataUrl;
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["appFk"] = appFk;
            query["username"] = username;
            uriBuilder.Query = query.ToString() ?? string.Empty;
            var data = await HttpService.http.GetAsJson<HttpResponse<List<UserDataDto>>>(uriBuilder.Uri, (req) => AddHeaders(req, PropUserData));
            if (data.IsSuccessful)
            {
                return data.Value.Object.FirstOrDefault();
            }
            return null;
        }

        private async Task<List<MenuDto>> GetUserMenu(string uid, string AppMenuUrl)
        {

            UriBuilder uriBuilder = new UriBuilder(this.MadamUrl);
            uriBuilder.Path = AppMenuUrl;
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["userAuthAppFK"] = uid;
            uriBuilder.Query = query.ToString() ?? string.Empty;
            try
            {
                var data = await HttpService.http.GetAsJson<HttpResponse<List<AppMenuDto>>>(uriBuilder.Uri, (req) => AddHeaders(req, PropUserMenu));
                if (data.IsSuccessful)
                {
                    var raw = data.Value.Object.Select(x => new MenuDto()
                    {
                        Name = x.Caption,
                        Key = x.Link,
                        Editable = x.Name.Contains("Edit"),
                        Alldata = x.Name.Contains("All Data"),
                        Create = x.Name.Contains("Create"),
                        Draft = x.Name.Contains("Draft"),
                        children = new()
                    }).ToList();

                    Dictionary<string, MenuDto> menus = new();
                    foreach(var appMenu in raw)
                    {
                        bool exist = menus.TryGetValue(appMenu.Name, out MenuDto? val);
                        if (exist && val != null)
                        {
                            val.Editable = val.Editable || appMenu.Editable;
                            val.Alldata = val.Alldata || appMenu.Alldata;
                            val.Create = val.Create || appMenu.Create;
                            val.Draft = val.Draft || appMenu.Draft;
                        }
                        else
                        {
                            menus.Add(appMenu.Name, appMenu);
                        }
                    }

                    return menus.Select(x => x.Value).ToList();
                }
            }
            catch(Exception ex)
            {
                return new List<MenuDto>();
            }

            return new List<MenuDto>();
        }

        public async Task<EmployeeDto?> GetEmployee(string username, string MasterEmployeeUrl)
        {

            UriBuilder uriBuilder = new UriBuilder(this.MadamUrl);
            uriBuilder.Path = MasterEmployeeUrl;
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["WhereCondition"] = $"EmpAccount='{username}'";
            uriBuilder.Query = query.ToString() ?? string.Empty;
            var data = await HttpService.http.GetAsJson<HttpResponse<List<EmployeeDto>>>(uriBuilder.Uri, (req) => AddHeaders(req, PropEmployee));
            if (data.IsSuccessful)
            {
                return data.Value.Object.FirstOrDefault();
            }
            return new EmployeeDto();
        }

        public async Task<List<EmployeeDto>> GetUserSuggestion(string user)
        {
            await GetBaseUrlListAsync();
            var employeeUrl = _parameterListRepository.GetParam("idams", "Url", "GetAllMasterEmployee").Result!.ParamValue1Text;
            UriBuilder uriBuilder = new UriBuilder(this.MadamUrl);
            uriBuilder.Path = employeeUrl; 
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["WhereCondition"] = $"EmpEmail like '%{user}%' or EmpName like '%{user}%'";
            uriBuilder.Query = query.ToString() ?? string.Empty;
            try
            {
                var data = await HttpService.http.GetAsJson<HttpResponse<List<EmployeeDto>>>(uriBuilder.Uri, (req) => AddHeaders(req, PropEmployee));
                if (data.IsSuccessful)
                {
                    var distinctRes = data.Value.Object.DistinctBy(n => n.EmpEmail).ToList();
                    return distinctRes;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error targetting : {uriBuilder.Uri}");
            }

            return new List<EmployeeDto>
            {
                new EmployeeDto()
                {
                    EmpAccount = "k2demo1", EmpEmail = "k2demo1@pertamina.com", EmpName = "K2 demo 1"
                },
                new EmployeeDto()
                {
                    EmpAccount = "k2demo2", EmpEmail = "k2demo2@pertamina.com", EmpName = "K2 demo 2"
                }
            };
        }

        public async Task GetBaseUrlListAsync()
        {
            var listParam = new List<ParamFilter>()
                {
                    new ParamFilter("idams","Url","BaseUrl"),
                    new ParamFilter("idams", "AppFK", "PropUserData"),
                    new ParamFilter("idams", "AppFK", "PropUserMenu"),
                    new ParamFilter("idams", "AppFK", "PropEmployee"),
                    new ParamFilter("idams", "AppFK", "token")
                };
            var paramList = await _parameterListRepository.GetParams(listParam);
            this.MadamUrl = paramList.Where(x => x.Schema == "idams" && x.ParamId == "Url" && x.ParamListId == "BaseUrl")
                .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
            this.PropUserData = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "PropUserData")
                .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
            this.PropUserMenu = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "PropUserMenu")
                .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
            this.PropEmployee = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "PropEmployee")
                .Select(x => x.ParamValue1Text).FirstOrDefault() ?? "";
            var token = paramList.Where(x => x.Schema == "idams" && x.ParamId == "AppFK" && x.ParamListId == "token")
                .Select(x => x.ParamListDesc).FirstOrDefault() ?? "";
            this.MadamToken = token ?? "";
        }

        private void AddHeaders(HttpRequestMessage request, string prop)
        {
            request.Headers.Add("X-User-Prop", prop);
            request.Headers.Add("X-User-Auth", MadamToken);
        }

    }
}
