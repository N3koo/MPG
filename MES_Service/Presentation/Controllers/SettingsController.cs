using MpgWebService.Repository.Interface;
using MpgWebService.DTO;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase {

        private readonly ISettingsRepository repository;

        public SettingsController(ISettingsRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<SettingsElement>>> GetSettings() {
            var result = await repository.GetSettings();

            if (result.Count == 0) {
                return NoContent();
            }

            return result;
        }

        [HttpPut]
        public async Task<ActionResult<bool>> SetSettings([FromBody] List<SettingsElement> settings) {
            var result = await repository.SetSettings(settings);

            if (!result) {
                return NoContent();
            }

            return result;
        }
    }
}
