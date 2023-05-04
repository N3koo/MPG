using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Service;

using System.Collections.Generic;
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

        [HttpGet]
        public async Task<IActionResult> GetPail() {
            var result = await service.GetAvailablePail();
            return Ok(result);
        }

        [HttpGet("{POID}")]
        public async Task<IActionResult> GetMaterials(string POID) {
            var result = await service.GetMaterials(POID);
            return Ok(result);
        }


        [HttpGet("Steps/{POID}")]
        public async Task<IActionResult> GetOperations(string POID) {
            var result = await service.GetOperationsList(POID);
            return Ok(result);
        }

        [HttpPost("QC")]
        public async Task<IActionResult> SetQCStatus(QcDetails details) {
            var result = await service.SetQcStatus(details);
            return Ok(result);
        }

        [HttpGet("Correction")]
        public async Task<IActionResult> GetCorrection([FromQuery] QcDetails details) {
            var result = await service.GetCorrections(details);
            return Ok(result);
        }

        [HttpPut("Correction")]
        public async Task<IActionResult> SaveCorrectionMaterials([FromBody] POCorrection correction) {
            var result = await service.SaveCorrection(correction);
            return Ok(result);
        }

        [HttpPut("Materials")]
        public async Task<IActionResult> SaveDosageMaterials([FromBody] List<POConsumption> materials) {
            var result = await service.SaveDosageMaterials(materials);
            return Ok(result);
        }

        [HttpPost("{POID}/{pail}")]
        public async Task<IActionResult> SetPailStatus(string POID, string pail, [FromBody] string status) {
            var result = await service.ChangeStatus(POID, pail, status);
            return Ok(result.Message);
        }

    }
}
