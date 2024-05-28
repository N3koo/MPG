using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Request.Command;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IProductionService {

        public Task<IEnumerable<ProductionDto>> GetProductionStatus(Period period);

    }
}
