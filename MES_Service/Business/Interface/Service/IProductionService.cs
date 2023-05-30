using MpgWebService.Presentation.Request;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Business.Interface.Service {
    public interface IProductionService {
        public Task<IEnumerable<Production>> GetProductionStatus(Period period);
    }
}
