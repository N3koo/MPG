using MES_Service.DataExtensions;
using MES_Service.Interface;
using MES_Service.DTO;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using Microsoft.AspNetCore.Mvc;

namespace MES_Service.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase {

        private readonly ICommandRepository repository;

        public CommandController(ICommandRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ProductionOrderDto> GetCommands(DateTime startDate, DateTime endDate) {
            return repository.GetCommands(startDate, endDate).Select(item => item.AsDto());
        }

        [HttpGet("{POID}")]
        public ActionResult<ProductionOrderDto> GetCommand(string POID) {
            var item = repository.GetCommands(POID);

            if (item == null) {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPut("start/{POID}")]
        public ActionResult<int> StartCommand(string POID, [FromBody] QcDto qc) {
            repository.StartCommand(POID);

            qc.Qc.ForEach(item => Debug.WriteLine(item));
            return 1;
        }

        [HttpGet("qc/{POID}")]
        public ActionResult<string> GetQC(string POID) {
            var result = repository.GetQC(POID);

            if (result == null) {
                return NotFound();
            }

            return result;
        }

        [HttpGet("priority/{priority}")]
        public ActionResult<bool> CheckPriority(string priority) {
            return repository.CheckPriority(priority);
        }

        [HttpPut("block/{POID}")]
        public ActionResult<bool> BlockCommand(string POID) {
            return repository.BlockCommand(POID);

        }

        [HttpPut("close/{POID}")]
        public ActionResult<bool> CloseCommand(string POID) {
            return repository.CloseCommand(POID);
        }

        [HttpPut("partial/{POID}")]
        public ActionResult<bool> PartialProduction(string POID) {
            return repository.PartialProduction(POID);
        }

        [HttpPost("materials")]
        public ActionResult<bool> DownloadMaterials() {
            return repository.DownloadMaterials();
        }
    }
}
