using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IProductionService {

        public Task<ServiceResponse<IList<ProductionDto>>> GetProductionStatus(Period period);

    }
}
