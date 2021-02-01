using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WikimediaProcessor.Data;
using WikimediaProcessor.Data.Repositories;
using WikimediaProcessor.Runners;
using WikimediaProcessor.Services.Downloaders;
using WikimediaProcessor.Services.Processors;
using WikimediaProcessor.Services.Readers;
using WikimediaProcessor.Services.Reporters;

namespace WikimediaProcessor
{
    public static class Register
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddDbContextFactory<WikimediaContext>(options =>
                options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"),
                oa => oa.EnableRetryOnFailure().CommandTimeout(180)));

            services.AddHttpClient<IPageViewsDownloader, PageViewsDownloader>(client =>
                client.BaseAddress = new Uri(context.Configuration["AppSettings:WikimediaBaseUrl"]));

            services.AddScoped<IWikimediaProcessor, Services.Processors.WikimediaProcessor>();
            services.AddScoped<IPageViewsReader, PageViewsReader>();
            services.AddScoped<IDomainCodeReader, DomainCodeReader>();
            services.AddScoped<IActivityPageRepository, ActivityPageRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ICsvReporter, CsvReporter>();
            services.AddScoped<IReportRunner, ReportRunner>();
            services.AddScoped<IProcessorRunner, ProcessorRunner>();
        }
    }
}
