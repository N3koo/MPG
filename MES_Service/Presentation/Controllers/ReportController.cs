using MpgWebService.Repository.Interface;
using MpgWebService.DTO;

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase {

        private readonly IReportRepository repository;

        public ReportController(IReportRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ReportCommand>> GetReport(DateTime start, DateTime end) {
            return await repository.GetReport(start, end);
        }

        [HttpGet("Materials/{POID}")]
        public async Task<IEnumerable<ReportMaterial>> GetCommandMaterials([Required] string POID) {
            return await repository.GetMaterialsForCommand(POID);
        }

        [HttpGet("Materials/{POID}/{pail}")]
        public async Task<IEnumerable<ReportMaterial>> GetPailMaterials([Required] string POID, [Required] int pail) {
            return await repository.GetMaterialsForPail(POID, pail);
        }

    }
}
