using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Castle.Facilities.Logging;
using Hangfire;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TalentV2.Configuration;
using TalentV2.Constants.Const;
using TalentV2.EntityFrameworkCore;
using TalentV2.FileServices.Paths;
using TalentV2.FileServices.Providers;
using TalentV2.Identity;
using TalentV2.WebServices;
using TalentV2.WebServices.ExternalServices.Firebase;

namespace TalentV2.Web.Host.Startup
{
	public class Startup
	{
		private const string _defaultCorsPolicyName = "localhost";

		private const string _apiVersion = "v1";

		private readonly IConfigurationRoot _appConfiguration;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public Startup(IWebHostEnvironment env)
		{
			_hostingEnvironment = env;
			_appConfiguration = env.GetAppConfiguration();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			//MVC
			services.AddControllersWithViews(
				options => { options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute()); }
			).AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				};
			});

			IdentityRegistrar.Register(services);
			AuthConfigurer.Configure(services, _appConfiguration);

			services.AddSignalR();
			services.AddDbContext<TalentV2DbContext>(options =>
					options.UseNpgsql(_appConfiguration.GetConnectionString("Default")));

			// Configure CORS for angular2 UI
			services.AddCors(
				options => options.AddPolicy(
					_defaultCorsPolicyName,
					builder => builder
						.WithOrigins(
							// App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
							_appConfiguration["App:CorsOrigins"]
								.Split(",", StringSplitOptions.RemoveEmptyEntries)
								.Select(o => o.RemovePostFix("/"))
								.ToArray()
						)
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials()
				)
			);
			// Swagger - Enable this line and the related lines in Configure method to enable swagger UI
			ConfigureSwagger(services);

			// Configure Abp and Dependency Injection
			services.AddAbpWithoutCreatingServiceProvider<TalentV2WebHostModule>(
				// Configure Log4Net logging
				options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
					f => f.UseAbpLog4Net().WithConfig(_hostingEnvironment.IsDevelopment()
						? "log4net.config"
						: "log4net.Production.config"
					)
				)
			);

			services.AddWebServices(_appConfiguration);

			services.AddHangfire(config =>
				config.UsePostgreSqlStorage(c =>
						c.UseNpgsqlConnection(_appConfiguration.GetConnectionString("Default"))));
			services.AddHangfireServer();

			services.Configure<FirebaseConfig>(_appConfiguration.GetSection("FirebaseConfig"));
			RegisterFileService(services);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

			app.UseCors(_defaultCorsPolicyName); // Enable CORS!

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAbpRequestLocalization();

			app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
			{
				DashboardTitle = "Admin",
				Authorization = new[]
				{
					new HangfireCustomBasicAuthenticationFilter{
						User = _appConfiguration.GetSection("HangfireConfig:UserName").Value,
						Pass = _appConfiguration.GetSection("HangfireConfig:Password").Value
					}
				},
				AppPath = null
			});

			app.UseHangfireDashboard("/public/hangfire", new DashboardOptions
			{
				DashboardTitle = "Public",
				Authorization = new[] { new HangfireAuthentication(_appConfiguration, loggerFactory) },
				AppPath = null
			});
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<AbpCommonHub>("/signalr");
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
			});

			// Enable middleware to serve generated Swagger as a JSON endpoint
			app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

			// Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
			app.UseSwaggerUI(options =>
			{
				// specifying the Swagger JSON endpoint.
				options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"TalentV2 API {_apiVersion}");
				options.IndexStream = () => Assembly.GetExecutingAssembly()
					.GetManifestResourceStream("TalentV2.Web.Host.wwwroot.swagger.ui.index.html");
				options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.  
			}); // URL: /swagger
		}

		private void ConfigureSwagger(IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc(_apiVersion, new OpenApiInfo
				{
					Version = _apiVersion,
					Title = "TalentV2 API",
					Description = "TalentV2",
					// uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "TalentV2",
						Email = string.Empty,
						Url = new Uri("https://twitter.com/aspboilerplate"),
					},
					License = new OpenApiLicense
					{
						Name = "Hangfire Monitoring",
						Url = new Uri(_appConfiguration["HangfireConfig:Root"] + "/admin/hangfire"),
					}
				});
				options.DocInclusionPredicate((docName, description) => true);

				// Define the BearerAuth scheme that's in use
				options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
				{
					Description =
						"JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey
				});

				//add summaries to swagger
				bool canShowSummaries = _appConfiguration.GetValue<bool>("Swagger:ShowSummaries");
				if (canShowSummaries)
				{
					var hostXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
					var hostXmlPath = Path.Combine(AppContext.BaseDirectory, hostXmlFile);
					options.IncludeXmlComments(hostXmlPath);

					var applicationXml = $"TalentV2.Application.xml";
					var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
					options.IncludeXmlComments(applicationXmlPath);

					var webCoreXmlFile = $"TalentV2.Web.Core.xml";
					var webCoreXmlPath = Path.Combine(AppContext.BaseDirectory, webCoreXmlFile);
					options.IncludeXmlComments(webCoreXmlPath);
				}
			});
		}
		private void RegisterFileService(IServiceCollection services)
		{
			LoadUploadFileConfig();
			if (TalentConstants.UploadFileProvider == TalentConstants.AmazoneS3)
			{
				CreateAWSCredentialProfile();
				services.AddAWSService<IAmazonS3>();
				services.AddTransient<IFileProvider, AWSProvider>();
				services.AddTransient<IFilePath, AWSFilePath>();
			}
			else
			{
				services.AddTransient<IFileProvider, InternalProvider>();
				services.AddTransient<IFilePath, InternalFilePath>();
			}

		}
		void CreateAWSCredentialProfile()
		{
			var options = new CredentialProfileOptions
			{
				AccessKey = AmazoneS3Constant.AccessKeyId,
				SecretKey = AmazoneS3Constant.SecretKeyId
			};
			var profile = new CredentialProfile(AmazoneS3Constant.Profile, options);
			profile.Region = RegionEndpoint.GetBySystemName(AmazoneS3Constant.Region);

			var sharedFile = new SharedCredentialsFile();
			sharedFile.RegisterProfile(profile);
		}
		private void LoadUploadFileConfig()
		{
			AmazoneS3Constant.Profile = _appConfiguration.GetValue<string>("AWS:Profile");
			AmazoneS3Constant.AccessKeyId = _appConfiguration.GetValue<string>("AWS:AccessKeyId");
			AmazoneS3Constant.SecretKeyId = _appConfiguration.GetValue<string>("AWS:SecretKeyId");
			AmazoneS3Constant.Region = _appConfiguration.GetValue<string>("AWS:Region");
			AmazoneS3Constant.BucketName = _appConfiguration.GetValue<string>("AWS:BucketName");
			AmazoneS3Constant.Prefix = _appConfiguration.GetValue<string>("AWS:Prefix");
			AmazoneS3Constant.CloudFront = _appConfiguration.GetValue<string>("AWS:CloudFront");

			TalentConstants.UploadFileProvider = _appConfiguration.GetValue<string>("UploadFile:Provider");
			TalentConstants.MaxSizeFile = _appConfiguration.GetValue<long>("UploadFile:MaxSizeFile");

			TalentConstants.LMSClientRootAddress = _appConfiguration.GetValue<string>("LMSService:FEAddress");

			TalentConstants.BaseBEAddress = _appConfiguration.GetSection("App")["ServerRootAddress"];
			TalentConstants.BaseFEAddress = _appConfiguration.GetSection("App")["ClientRootAddress"];
			TalentConstants.PublicClientRootAddress = _appConfiguration.GetSection("App")["PublicClientRootAddress"];
		}
	}
}
