using Microsoft.AspNetCore.Mvc;

namespace MES_Service.Controllers {

    [ApiController]
    [Route("[Controller]")]
    public class MpgController : ControllerBase {

        [HttpGet]
        public ActionResult<string> GetMPG() {
            return "Salut";
        }

    }
}
