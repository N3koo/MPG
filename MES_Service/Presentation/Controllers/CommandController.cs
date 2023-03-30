using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase {

        private readonly ICommandService service;

        public CommandController() {
            service = new CommandService();
        }

        [HttpGet]
        public async Task<IActionResult> GetCommands([FromQuery] Period period) {
            var result = await service.GetCommands(period);
            return Ok(result);
        }

        [HttpGet("{POID}")]
        public async Task<IActionResult> GetCommand(string POID) {
            var result = await service.GetCommand(POID);
            return Ok(result);
        }

        [HttpPut("Start")]
        public async Task<IActionResult> StartCommand([FromBody] StartCommand details) {
            var result = await service.StartCommand(details);
            return Ok(result.Message);
        }

        [HttpGet("QC/{POID}")]
        public async Task<IActionResult> GetQC(string POID) {
            var result = await service.GetQC(POID);
            return Ok(result);
        }

        [HttpGet("Priority/{priority}")]
        public async Task<IActionResult> CheckPriority(string priority) {
            var result = await service.CheckPriority(priority);
            return Ok(result);
        }

        [HttpDelete("Block/{POID}")]
        public async Task<IActionResult> BlockCommand(string POID) {
            var result = await service.BlockCommand(POID);
            return Ok(result);
        }

        [HttpPut("Close/{POID}")]
        public async Task<IActionResult> CloseCommand(string POID) {
            var result = await service.CloseCommand(POID);
            return Ok(result);
        }

        [HttpPut("Partial/{POID}")]
        public async Task<IActionResult> PartialProduction(string POID) {
            var result = await service.StartPartialProduction(POID);

            return Ok(result.Message);
        }

        [HttpPost("Materials")]
        public async Task<ActionResult<bool>> DownloadMaterials() {
            var result = await service.DownloadMaterials();
            return Ok(result.Message);
        }
    }
}
