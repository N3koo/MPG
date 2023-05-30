using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[controller]")]
    [Produces("text/json")]
    public class ProductionController : ControllerBase {

        private readonly IProductionService service;

        public ProductionController() {
            service = new ProductionService();
        }

        [HttpGet]
        public async Task<IActionResult> GetResult([FromQuery] Period period) =>            Ok(await service.GetProductionStatus(period));    }
}
