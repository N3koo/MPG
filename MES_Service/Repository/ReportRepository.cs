using MES_Service.Interface;
using MES_Service.DTO;

using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

using NHibernate.Transform;

namespace MES_Service.Repository {

    public class ReportRepository : IReportRepository {

        private readonly string QueryCommand = "SELECT pc.item, md.Description, SUM(pc.itemQty) AS NetQuantity, pc.ItemUom " +
               "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
               "WHERE pc.POID = ? GROUP BY pc.Item;";

        private readonly string QueryPail = "SELECT pc.item, md.Description, pc.itemQty AS NetQuantity, pc.ItemUom " +
                "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
                "WHERE pc.POID = ? AND pc.PailNumber = ? GROUP BY pc.Item;";

        public IEnumerable<ReportCommandDto> GetReport(DateTime start, DateTime end) {
            List<ReportCommandDto> dtos = new();

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start && p.PlannedEndDate <= end).ToList();

                    if (result.Count == 0) {
                        return dtos;
                    }

                    result.ForEach(item => {
                        dtos.Add(ReportCommandDto.CreateFromPO(item));

                        var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList();
                        details.ForEach(pail => {
                            dtos.Add(ReportCommandDto.CreateFromPailStatus(pail, item));
                        });

                    });

                    return dtos;
                }
            }
        }

        public IEnumerable<ReportMaterialDto> GetMaterialsForCommand(string POID) {
            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    return session.CreateSQLQuery(QueryCommand)
                        .SetResultTransformer(Transformers.AliasToBean<ReportMaterialDto>())
                        .SetString(0, POID).List<ReportMaterialDto>();
                }
            }
        }

        public IEnumerable<ReportMaterialDto> GetMaterialsForPail(string POID, int pail) {
            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    return session.CreateSQLQuery(QueryPail)
                        .SetResultTransformer(Transformers.AliasToBean<ReportMaterialDto>())
                        .SetString(0, POID)
                        .SetInt32(1, pail)
                        .List<ReportMaterialDto>();
                }
            }
        }
    }
}
