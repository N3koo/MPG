using Microsoft.AspNetCore.Mvc;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.Command;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetResult([FromQuery] Period period) =>
            Ok(await service.GetProductionStatus(period));

    }
}
