using MpgWebService.Repository.Interface;
using MpgWebService.DTO;

using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Presentation.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase {

        private readonly IProductionRepository repository;

        public ProductionController(IProductionRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<Production>> GetResult([Required] DateTime start, [Required] DateTime end) {
            return await repository.CheckProductionStatus(start, end);
        }
    }
}
