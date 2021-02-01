using System.Threading.Tasks;

namespace WikimediaProcessor.Services.Processors
{
    public interface IWikimediaProcessor
    {
        Task RunAsync(string startYear, string startMonth, string endYear, string endMonth);
    }
}