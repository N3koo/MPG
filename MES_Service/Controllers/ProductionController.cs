using MES_Service.DTO;
using MES_Service.Interface;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MES_Service.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase {

        private readonly IProductionRepository repository;

        public ProductionController(IProductionRepository repository) {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<ProductionDto> GetResult([Required] DateTime start, [Required] DateTime end) {
            return repository.CheckProductionStatus(start, end);
        }
    }
}
