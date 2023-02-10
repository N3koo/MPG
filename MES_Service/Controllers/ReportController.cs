using Microsoft.AspNetCore.Mvc;

using MES_Service.DTO;

using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using MES_Service.Interface;

namespace MES_Service.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase {

        private readonly IReportRepository repository;

        public ReportController(IReportRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ReportCommandDto> GetReport([Required] DateTime start, [Required] DateTime end) {
            return repository.GetReport(start, end);
        }

        [HttpGet("materials/{POID}")]
        public IEnumerable<ReportMaterialDto> GetCommandMaterials([Required] string POID) {
            return repository.GetMaterialsForCommand(POID);
        }

        [HttpGet("materials/{POID}/{pail}")]
        public IEnumerable<ReportMaterialDto> GetPailMaterials([Required] string POID, [Required] int pail) {
            return repository.GetMaterialsForPail(POID, pail);
        }

    }
}
