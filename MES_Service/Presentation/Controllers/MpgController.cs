using MpgWebService.Repository.Interface;

using Microsoft.AspNetCore.Mvc;

namespace MpgWebService.Presentation.Controllers {

    [ApiController]
    [Route("[Controller]")]
    public class MpgController : ControllerBase {

        private readonly IMpgRepository repository;

        public MpgController(IMpgRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<string> GetMPG() {
            return "Salut";
        }

        [HttpGet("{status}")]
        public ActionResult<string> GetCommandsByStatus(string status) {
            return string.Empty;
        }

    }
}
