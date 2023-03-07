using MpgWebService.Repository.Interface;
using MpgWebService.Data.Extension;
using MpgWebService.DTO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System;

using Microsoft.AspNetCore.Mvc;
using MpgWebService.Data.Wrappers;
using MpgWebService.Data.Filter;
using MpgWebService.Business.Service;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase {

        private readonly ICommandService service;

        public CommandController(ICommandService service) {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommands(DateTime start, DateTime end, [FromQuery] PaginationFilter filter) {
            var result = await repository.GetCommands(start, end);
            result = result.Skip(filter.PageNumber - 1 * filter.PageSize)
                .Take(filter.PageSize).ToList();

            return Ok(result.Select(item => new Response<ProductionOrderDto>(item.AsDto())));
        }

        [HttpGet("{POID}")]
        public async Task<IActionResult> GetCommand(string POID) {
            var item = await repository.GetCommand(POID);

            if (item == null) {
                return NotFound();
            }

            return Ok(new Response<ProductionOrderDto>(item.AsDto()));
        }

        [HttpPut("Start/{POID}")]
        public ActionResult<int> StartCommand(string POID, [FromBody] QcDto qc) {
            throw new NotImplementedException();
            repository.StartCommand(POID);

            qc.Qc.ForEach(item => Debug.WriteLine(item));
            return 1;
        }

        [HttpGet("QC/{POID}")]
        public async Task<ActionResult<string>> GetQC(string POID) {
            var result = await repository.GetQC(POID);

            if (result == null) {
                return NotFound();
            }

            return result;
        }

        [HttpGet("Priority/{priority}")]
        public async Task<ActionResult<bool>> CheckPriority(string priority) {
            return await repository.CheckPriority(priority);
        }

        [HttpDelete("Block/{POID}")]
        public async Task<ActionResult<bool>> BlockCommand(string POID) {
            return await repository.BlockCommand(POID);

        }

        [HttpPut("Close/{POID}")]
        public async Task<ActionResult<bool>> CloseCommand(string POID) {
            return await repository.CloseCommand(POID);
        }

        [HttpPut("Partial/{POID}")]
        public async Task<ActionResult<bool>> PartialProduction(string POID) {
            return await repository.PartialProduction(POID);
        }

        [HttpPost("Materials")]
        public async Task<ActionResult<bool>> DownloadMaterials() {
            string date = Properties.Settings.Default.Update;

            if (string.IsNullOrEmpty(date)) {
                return await repository.DownloadMaterials();
            } else {
                return await repository.UpdateMaterials();
            }
        }
    }
}
