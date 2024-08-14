using Idams.Core.Enums;
using AutoMapper;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.EntityFramework;
using Idams.Infrastructure.EntityFramework.Configurations;
using Idams.Infrastructure.EntityFramework.Repositories;
using Idams.Infrastructure.Services;
using Idams.Infrastructure.Utils;
using Idams.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Idams.Core.Extenstions;

namespace System
{
    public static class AppExtensions
    {
        public static IServiceCollection AddIdams(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdamsDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Idams.Infrastructure.EntityFramework"));
            });

            AddKeyClock(services, configuration);

            services.AddDataProtection();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddScoped<IUnitOfWorks, UnitOfWorks>();
            services.AddMemoryCache();
            AddRepositories(services);
            AddUtils(services);
            AddServices(services);
            services.AddAutoMapper(c =>
            {
                ConfigureAutoMapper(c);
            });
            services.AddScoped<IMappingObject, MappingObject>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Jwt", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            return services;
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IWorkflowSequenceService, WorkflowSequenceService>();
            services.AddScoped<IHierLevelService, HierLevelService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOutstandingTaskListService, OutstandingTaskListService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IWorkflowIntegrationService, WorkflowIntegrationService>();
        }

        private static void AddUtils(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, EFUserRepository>();
            services.AddScoped<IParameterListRepository, EFParameterListRepository>();
            services.AddScoped<IHierLvlVwRepository, EFHierLvlVwRepository>();
            services.AddScoped<IRefTemplateWorkflowSequenceRepository, EFRefTemplateWorkflowSequenceRepository>();
            services.AddScoped<IRefWorkflowRepository, EFRefWorkflowRepository>();
            services.AddScoped<IWorkflowSettingRepository, EFWorkflowSettingRepository>();
            services.AddScoped<IDocumentRepository, EFDocumentRepository>();
            services.AddScoped<IDocChecklistRepository, EFDocChecklistRepository>();
            services.AddScoped<IProjectRepository, EFProjectRepository>();
            services.AddScoped<IProjectActionRepository, EFProjectActionRepository>();
            services.AddScoped<ITableColumnSettingRepository, EFTableColumnSettingRepository>();
            services.AddScoped<IProjectAuditTrailRepository, EFProjectAuditTrail>();
            services.AddScoped<IProjectUpstreamEntityRepository, EFProjectUpstreamEntityRepository>();
            services.AddScoped<IProjectPlatformRepository, EFProjectPlatformRepository>();
            services.AddScoped<IProjectPipelineRepository, EFProjectPipelineRepository>();
            services.AddScoped<IProjectCompressorRepository, EFProjectCompressorRepository>();
            services.AddScoped<IProjectEquipmentRepository, EFProjectEquipmentRepository>();
            services.AddScoped<ITemplateDocumentRepository, EFRefTemplateDocumentRepository>();
            services.AddScoped<IProjectMilestoneRepository, EFProjectMilestoneRepository>();
            services.AddScoped<ILgProjectActivityAuditTrailRepository, EFLgProjectActivityAuditTrailRepository>();
            services.AddScoped<IApprovalRepository, EFApprovalRepository>();
            services.AddScoped<IFollowUpRepository, EFFollowUpRepository>();
            services.AddScoped<ITxMeetingRepositories, EFTxMeetingRepositories>();
            services.AddScoped<ITxMeetingParticipantRepository, EFTxMeetingParticipantRepository>();
            services.AddScoped<IProjectCommentRepository, EFProjectCommentRepository>();
            services.AddScoped<IDocumentManagementRepository, EFDocumentManagementRepository>();
            services.AddScoped<IUpcomingMeetingRepository, EFUpcomingMeetingRepository>();
            services.AddScoped<IOutstandingTaskListRepository, EFOutstandingTaskListRepository>();
            services.AddScoped<IFidcodeRepository, EFFidcodeRepository>();
            services.AddScoped<IUnitOfWorks, UnitOfWorks>();
        }
        private static void ConfigureAutoMapper(IMapperConfigurationExpression configuration)
        {
            configuration.AddProfile<AutoMapperConfiguration>();
        }
        #region KeyClock

        private static void AddKeyClock(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                //Sets cookie authentication scheme
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })

                         .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookie =>
                         {
                             //Sets the cookie name and maxage, so the cookie is invalidated.
                             cookie.Cookie.Name = "keycloak.cookie";
                             cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                             cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                             cookie.SlidingExpiration = true;
                         })
                         .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                         {
                             options.TokenValidationParameters = new TokenValidationParameters
                             {
                                 ValidateAudience = true,
                                 ValidateLifetime = true,
                                 ValidateIssuer = true,
                                 ValidIssuer = configuration.GetSection("Keycloak")["ServerRealm"],
                                 ValidAudience = configuration.GetSection("Keycloak")["ValidAudience"],
                                 SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                                 {
                                     var jwt = new JwtSecurityToken(token);
                                     return jwt;
                                 },
                             };
                             options.Events = new JwtBearerEvents
                             {
                                 OnAuthenticationFailed = async (context) =>
                                 {
                                     Console.WriteLine("Printing in the delegate OnAuthFailed");
                                 },
                                 OnChallenge = async (context) =>
                                 {
                                     // Ensure we always have an error and error description.
                                     if (string.IsNullOrEmpty(context.Error))
                                         context.Error = "invalid_token";
                                     if (string.IsNullOrEmpty(context.ErrorDescription))
                                         context.ErrorDescription = "This request requires a valid JWT access token to be provided";


                                     if (context.AuthenticateFailure != null)
                                     {
                                         context.Response.ContentType = "application/json";
                                         var token = context.Request.Headers.Authorization;
                                         context.Response.StatusCode = 401;
                                         if (token == Microsoft.Extensions.Primitives.StringValues.Empty)
                                         {
                                             await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                                             {
                                                 errorCode = ErrorCode.TokenEmpty,
                                                 status = "Failed",
                                                 message = ResultCodeMessages.GetResultCode(ErrorCode.TokenEmpty),
                                             }));
                                             return;
                                         }
                                         var currentToken = token.ToString().Split(' ')[1];
                                         if (JwtUtils.isEmptyOrInvalid(currentToken))
                                         {
                                             await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                                             {
                                                 errorCode = ErrorCode.ExpiredToken,
                                                 status = "Failed",
                                                 message = ResultCodeMessages.GetResultCode(ErrorCode.ExpiredToken),
                                             }));
                                         }
                                         else
                                         {
                                             await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                                             {
                                                 errorCode = ErrorCode.UnHandleUnAuthorizeException,
                                                 status = "Failed",
                                                 message = context.AuthenticateFailure.Message,
                                             }));
                                         }
                                     }
                                 }
                             };
                         })
                         .AddOpenIdConnect(options =>
                         {
                             options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                             //Keycloak server
                             options.Authority = configuration.GetSection("Keycloak")["ServerRealm"];
                             //Keycloak client ID
                             options.ClientId = configuration.GetSection("Keycloak")["ClientId"];
                             //Keycloak client secret in user secrets for dev
                             options.ClientSecret = configuration.GetSection("Keycloak")["ClientSecret"];
                             //Keycloak .wellknown config origin to fetch config
                             options.MetadataAddress = configuration.GetSection("Keycloak")["Metadata"];
                             //Require keycloak to use SSL

                             options.GetClaimsFromUserInfoEndpoint = true;
                             options.Scope.Add("openid");
                             options.Scope.Add("profile");
                             options.SaveTokens = true;
                             options.ResponseType = OpenIdConnectResponseType.Code;
                             options.RequireHttpsMetadata = false; //dev

                             options.TokenValidationParameters = new TokenValidationParameters
                             {
                                 NameClaimType = "name",
                                 RoleClaimType = ClaimTypes.Role,
                                 ValidateIssuer = true
                             };
                         });
        }

        #endregion
    }
}
