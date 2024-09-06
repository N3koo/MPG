using Microsoft.AspNetCore.Mvc;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Command;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class ProductionController : ControllerBase {

        private readonly IProductionService service;

        public ProductionController(IProductionService service) {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetResult([FromQuery] Period period) =>
            Ok(await service.GetProductionStatus(period));

    }
}
