using System.Threading.Tasks;

namespace WikimediaProcessor.Runners
{
    public interface IProcessorRunner
    {
        Task RunAsync();
    }
}
