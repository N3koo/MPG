using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using MES_Service.Interface;
using MES_Service.DTO;

using System.Collections.Generic;
using System.Linq;
using System;

namespace MES_Service.Repository {

    public class ProductionRepository : IProductionRepository {

        public IEnumerable<ProductionDto> CheckProductionStatus(DateTime start, DateTime end) {
            List<ProductionDto> dtos = new();

            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();
            var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start && p.PlannedEndDate <= end).ToList();

            if (result.Count == 0) {
                return dtos;
            }

            result.ForEach(item => {
                dtos.Add(ProductionDto.CreateFromPO(item));

                var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList();
                details.ForEach(pail => {
                    dtos.Add(ProductionDto.CreateFromPailStatus(pail));
                });
            });

            return dtos;
        }
    }
}
