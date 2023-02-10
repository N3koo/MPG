using System.Collections.Generic;
using System;

using MES_Service.DTO;

namespace MES_Service.Interface {
    public interface IProductionRepository {
        IEnumerable<ProductionDto> CheckProductionStatus(DateTime start, DateTime end);
    }
}
