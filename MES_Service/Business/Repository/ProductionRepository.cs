using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Production;
using MpgWebService.Repository.Interface;
using MpgWebService.Data.Extension;

using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MpgWebService.Repository {

    public class ProductionRepository : IProductionRepository {

        public Task<List<ProductionDto>>  CheckProductionStatus(Period period) {
            List<ProductionDto> dtos = new();

            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();
            var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToList();

            if (result.Count == 0) {
                return Task.FromResult(dtos);
            }

            result.ForEach(item => {
                dtos.Add(item.AsProductionDto());

                var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID);
                dtos.AddRange(details.Select(p => p.AsProductionDto()));
            });

            return Task.FromResult(dtos);
        }
    }
}
