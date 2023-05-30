using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Business.Service {
    public class ProductionService : IProductionService {

        private readonly IProductionRepository repository;

        public ProductionService() {
            repository = new ProductionRepository();
        }

        public async Task<IEnumerable<Production>> GetProductionStatus(Period period) {
            return await repository.CheckProductionStatus(period);
        }
    }
}
