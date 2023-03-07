using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using MpgWebService.DTO;

namespace MpgWebService.Repository.Interface {

    public interface IProductionRepository {

        Task<List<Production>> CheckProductionStatus(DateTime start, DateTime end);
    }
}
