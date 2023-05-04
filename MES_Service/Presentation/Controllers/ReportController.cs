using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[controller]")]
    [Produces("text/json")]
    public class ReportController : ControllerBase {

        private readonly IReportService repository;

        public ReportController() {
            repository = new ReportService();
        }

        [HttpGet]
        public async Task<IEnumerable<ReportCommand>> GetReport([FromQuery] Period period) {
            return await repository.GetReport(period);
        }

        [HttpGet("Materials/{POID}")]
        public async Task<IEnumerable<ReportMaterial>> GetCommandMaterials(string POID) {
            return await repository.GetMaterialsForCommand(POID);
        }

        [HttpGet("Materials/{POID}/{pail}")]
        public async Task<IEnumerable<ReportMaterial>> GetPailMaterials(string POID, int pail) {
            return await repository.GetMaterialsForPail(POID, pail);
        }

    }
}
