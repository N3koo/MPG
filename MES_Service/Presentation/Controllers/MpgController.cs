using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class MpgController : ControllerBase {

        private readonly IMpgService service;

        public MpgController() {
            service = new MpgService();
        }

        [HttpGet("Pail")]
        public async Task<IActionResult> GetPail() =>
            Ok(await service.GetAvailablePail());

        [HttpGet("Labels/{POID}")]
        public async Task<IActionResult> GetLabel(string POID) =>
            Ok(await service.GetLabel(POID));

        [HttpGet("Materials/{POID}")]
        public async Task<IActionResult> GetMaterials(string POID) =>
            Ok(await service.GetMaterials(POID));

        [HttpPost("QC")]
        public async Task<IActionResult> SetQCStatus(QcDetails details) =>
            Ok(await service.GetQcLabel(details.POID, details.PailNumber));

        [HttpGet("Correction")]
        public async Task<IActionResult> GetCorrection([FromQuery] QcDetails details) =>
            Ok(await service.GetCorrections(details.POID, details.PailNumber, details.OpNo));

        [HttpPut("Correction")]
        public async Task<IActionResult> SaveCorrectionMaterials([FromBody] POConsumption materials) =>
            Ok(await service.SaveCorrection(materials));

        [HttpPut("Materials")]
        public async Task<IActionResult> SaveDosageMaterials([FromBody] POConsumption materials) => 
            Ok(await service.SaveDosageMaterials(materials));

        [HttpPost("{POID}/{pail}")]
        public async Task<IActionResult> SetPailStatus(string POID, string pail, [FromBody] string status) {
            var result = await service.ChangeStatus(POID, pail, status);
            return Ok(result.Message);
        }
    }
}
