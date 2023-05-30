using MpgWebService.Presentation.Request;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Repository.Interface {

    public interface IProductionRepository {

        Task<List<Production>> CheckProductionStatus(Period period);
    }
}
