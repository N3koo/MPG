using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace MpgWebService.Business.Service {

    public class ProductionService : IProductionService {

        private readonly IProductionRepository repository;

        public ProductionService(IProductionRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse<IList<ProductionDto>>> GetProductionStatus(Period period) =>
            await repository.CheckProductionStatus(period);

    }
}
