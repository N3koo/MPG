using Microsoft.AspNetCore.Mvc;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Service;
using MpgWebService.Presentation.Request.MPG;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class MpgController : ControllerBase {

        private readonly IMpgService service;

        public MpgController() {
            service = new MpgService();
        }

        [HttpGet("Pails/QC")]
        public async Task<IActionResult> GetQCPail() =>
            Ok(await service.GetQCPail());

        [HttpGet("Pails/{POID}")]
        public async Task<IActionResult> GetPail(string POID) =>
            Ok(await service.GetAvailablePail(POID));

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
        public async Task<IActionResult> SetPailStatus(string POID, string pail, [FromBody] string status) =>
            Ok(await service.ChangeStatus(POID, pail, status));

        [HttpGet("Coefficients")]
        public async Task<IActionResult> GetHeadsCoefficients() =>
            Ok(await service.GetCoefficients());

        [HttpPost("Reserver")]
        public async Task<IActionResult> SetReservedQuantity([FromBody] ReserveTank[] quantities) =>
            Ok(await service.UpdateReserveQuantities(quantities));
    }
}
