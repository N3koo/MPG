using Microsoft.AspNetCore.Mvc;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    [Produces("text/json")]
    public class SettingsController : ControllerBase {

        private readonly ISettingsService service;

        public SettingsController(ISettingsService service) {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings() =>
            Ok(await service.GetSettings());

        [HttpPut]
        public async Task<IActionResult> SetSettings([FromBody] List<SettingsElement> settings) =>
            Ok(await service.SetSettings(settings));

    }
}
