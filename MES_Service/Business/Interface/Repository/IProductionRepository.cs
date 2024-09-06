using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IProductionRepository {

        Task<ServiceResponse<IList<ProductionDto>>> CheckProductionStatus(Period period);
    }
}
