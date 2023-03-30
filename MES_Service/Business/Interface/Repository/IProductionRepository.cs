using MpgWebService.Presentation.Request;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IProductionRepository {

        Task<List<Production>> CheckProductionStatus(Period period);
    }
}
