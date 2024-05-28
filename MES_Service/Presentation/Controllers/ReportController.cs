using MpgWebService.Presentation.Request.Command;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class ReportController : ControllerBase {

        private readonly IReportService repository;

        public ReportController() {
            repository = new ReportService();
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] Period period) =>
            Ok(await repository.GetReport(period));

        [HttpGet("Materials/{POID}")]
        public async Task<IActionResult> GetCommandMaterials(string POID) =>
            Ok(await repository.GetMaterialsForCommand(POID));

        [HttpGet("Materials/{POID}/{pail}")]
        public async Task<IActionResult> GetPailMaterials(string POID, int pail) =>
            Ok(await repository.GetMaterialsForPail(POID, pail));

    }
}
