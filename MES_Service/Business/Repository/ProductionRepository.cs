using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using MpgWebService.Repository.Interface;
using MpgWebService.Data.Extension;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MpgWebService.Repository {

    public class ProductionRepository : IProductionRepository {

        public Task<List<Production>> CheckProductionStatus(DateTime start, DateTime end) {
            List<Production> dtos = new();

            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();
            var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start && p.PlannedEndDate <= end).ToList();

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
