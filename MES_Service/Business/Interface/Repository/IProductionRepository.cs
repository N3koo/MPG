using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Request.Command;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IProductionRepository {

        Task<List<ProductionDto>> CheckProductionStatus(Period period);
    }
}
