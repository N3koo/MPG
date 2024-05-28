using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace MpgWebService.Business.Service {

    public class ProductionService : IProductionService {

        private readonly IProductionRepository repository;

        public ProductionService() {
            repository = new ProductionRepository();
        }

        public async Task<IEnumerable<ProductionDto>> GetProductionStatus(Period period) =>
            await repository.CheckProductionStatus(period);

    }
}
