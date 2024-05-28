using MpgWebService.Presentation.Request.Command;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class CommandController : ControllerBase {

        private readonly ICommandService service;

        public CommandController() {
            service = new CommandService();
        }

        [HttpGet]
        public async Task<IActionResult> GetCommands([FromQuery] Period period) =>
            Ok(await service.GetCommands(period));

        [HttpGet("{POID}")]
        public async Task<IActionResult> GetCommand(string POID) =>
            Ok(await service.GetCommand(POID));

        [HttpPut("Start")]
        public async Task<IActionResult> StartCommand([FromBody] StartCommand details) =>
            Ok((await service.StartCommand(details)).Message);

        [HttpGet("QC/{POID}")]
        public async Task<IActionResult> GetQC(string POID) =>
            Ok(await service.GetQC(POID));

        [HttpGet("Priority/{priority}")]
        public async Task<IActionResult> CheckPriority(string priority) => 
            Ok(await service.CheckPriority(priority));

        [HttpDelete("Block/{POID}")]
        public async Task<IActionResult> BlockCommand(string POID) =>
            Ok((await service.BlockCommand(POID)).Message);

        [HttpPut("Close/{POID}")]
        public async Task<IActionResult> CloseCommand(string POID) =>
            Ok((await service.CloseCommand(POID)).Message);

        [HttpPut("Partial/{POID}")]
        public async Task<IActionResult> PartialProduction(string POID) =>
            Ok((await service.StartPartialProduction(POID)).Message);

        [HttpPost("Materials")]
        public async Task<ActionResult> DownloadMaterials() =>
            Ok((await service.DownloadMaterials()).Message);

    }
}
