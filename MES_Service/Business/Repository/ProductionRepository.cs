using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MpgWebService.Repository {

    public class ProductionRepository : IProductionRepository {

        public async Task<ServiceResponse<IList<ProductionDto>>> CheckProductionStatus(Period period) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();
                var result = await session.Query<ProductionOrder>()
                    .Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToListAsync();

                if (result.Count == 0) {
                    return ServiceResponse<IList<ProductionDto>>.NotFound("Nu exista date in perioada cautata");
                }

                var dtos = new List<ProductionDto>();
                result.ForEach(item => {
                    dtos.Add(item.AsProductionDto());

                    var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID);
                    dtos.AddRange(details.Select(p => p.AsProductionDto()));
                });

                return ServiceResponse<IList<ProductionDto>>.Ok(dtos);
            } catch (Exception ex) {
                return ServiceResponse<IList<ProductionDto>>.CreateErrorMpg(ex.Message);
            }
        }
    }
}
