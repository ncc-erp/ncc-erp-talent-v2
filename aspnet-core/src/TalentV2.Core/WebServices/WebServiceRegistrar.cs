using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentV2.WebServices.ExternalServices.Komu;
using TalentV2.WebServices.InternalServices.HRM;
using TalentV2.WebServices.InternalServices.LMS;

namespace TalentV2.WebServices
{
    public static class WebServiceRegistrar
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfigurationRoot _appConfiguration)
        {
            services.AddHttpClient<KomuService>(options =>
            {
                options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("KomuService:BaseAddress"));
                options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("KomuService:SecurityCode"));
            });
            services.AddHttpClient<LMSService>(options =>
            {
                options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("LMSService:BaseAddress"));
                options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("LMSService:SecurityCode"));
            });
            services.AddHttpClient<HRMService>(options =>
            {
                options.BaseAddress = new Uri(_appConfiguration.GetValue<string>("HRMService:BaseAddress"));
                options.DefaultRequestHeaders.Add("X-Secret-Key", _appConfiguration.GetValue<string>("HRMService:SecurityCode"));
            });
            return services;
        }
    }
}
