using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WikimediaProcessor.Services.Processors;

namespace WikimediaProcessor.Runners
{
    public class ProcessorRunner : IProcessorRunner
    {
        private readonly IConfiguration configuration;
        private readonly IWikimediaProcessor wikimediaProcessor;

        public ProcessorRunner(IConfiguration configuration, IWikimediaProcessor wikimediaProcessor)
        {
            this.configuration = configuration;
            this.wikimediaProcessor = wikimediaProcessor;
        }

        public Task RunAsync()
        {
            var startYear = configuration["AppSettings:StartYear"];
            var endYear = configuration["AppSettings:EndYear"];
            var startMonth = configuration["AppSettings:StartMonth"];
            var endMonth = configuration["AppSettings:EndMonth"];

            if (string.IsNullOrEmpty(startYear))
                throw new ArgumentNullException(nameof(startYear));
            if (string.IsNullOrEmpty(endYear))
                throw new ArgumentNullException(nameof(endYear));

            return wikimediaProcessor.RunAsync(startYear, startMonth, endYear, endMonth);
        }
    }
}
