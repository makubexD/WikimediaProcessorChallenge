using System.Collections.Generic;
using WikimediaProcessor.Data.Entities;
using System.Threading.Tasks;

namespace WikimediaProcessor.Data.Repositories
{
    public interface IPageAllRepository
    {
        void Create(List<PageAll> pageAll);
        void CreateByBatches(List<PageAll> pageAll);
    }
}
