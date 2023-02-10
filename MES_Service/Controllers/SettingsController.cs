using Microsoft.AspNetCore.Mvc;

namespace MES_Service.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase {

        [HttpGet]
        public string GetSettings() {
            return "SAlut";
        }
    }
}
