using Microsoft.AspNetCore.Mvc;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Command;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class ReportController : ControllerBase {

        private readonly IReportService repository;

        public ReportController(IReportService repository) {
            this.repository = repository;
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
