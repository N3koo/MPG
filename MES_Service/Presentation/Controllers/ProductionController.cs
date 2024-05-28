using MpgWebService.Presentation.Request.Command;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class ProductionController : ControllerBase {

        private readonly IProductionService service;

        public ProductionController() {
            service = new ProductionService();
        }

        [HttpGet]
        public async Task<IActionResult> GetResult([FromQuery] Period period) =>            Ok(await service.GetProductionStatus(period));    }
}
