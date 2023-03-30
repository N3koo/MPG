using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;


namespace MpgWebService.Presentation.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase {

        private readonly IProductionService service;

        public ProductionController() {
            service = new ProductionService();
        }

        [HttpGet]
        public async Task<IEnumerable<Production>> GetResult([FromQuery] Period period) {
            return await service.GetProductionStatus(period);
        }
    }
}
