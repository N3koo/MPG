using MpgWebService.Presentation.Request;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {
    public interface IProductionService {
        public Task<IEnumerable<Production>> GetProductionStatus(Period period);
    }
}
