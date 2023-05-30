using NHibernate.Transform;

using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Data.Extension;

using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Repository {

    public class ReportRepository : IReportRepository {

        private readonly string QueryCommand = "SELECT pc.item, md.Description, SUM(pc.itemQty) AS NetQuantity, pc.ItemUom " +
               "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
               "WHERE pc.POID = ? GROUP BY pc.Item, md.Description, pc.ItemUom;";

        private readonly string QueryPail = "SELECT pc.item, md.Description, pc.itemQty AS NetQuantity, pc.ItemUom " +
                "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
                "WHERE pc.POID = ? AND pc.PailNumber = ?";

        public Task<List<ReportCommand>> GetReport(Period period) {
            List<ReportCommand> dtos = new();

            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();
            var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToList();

            if (result.Count == 0) {
                return Task.FromResult(dtos);
            }

            result.ForEach(item => {
                dtos.Add(item.AsReportDto());

                var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList();
                dtos.AddRange(details.Select(p => p.AsReportDto(item)));
            });

            return Task.FromResult(dtos);
        }

        public Task<IList<ReportMaterial>> GetMaterialsForCommand(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var boms = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            var materials = session.CreateSQLQuery(QueryCommand)
                .SetResultTransformer(Transformers.AliasToBean<ReportMaterial>())
                .SetString(0, POID).List<ReportMaterial>();

            boms.ForEach(item => {
                var material = materials.First(p => p.Item == item.Item);
                material.BrutQuantity = item.ItemQty;
            });

            return Task.FromResult(materials);
        }

        public Task<IList<ReportMaterial>> GetMaterialsForPail(string POID, int pail) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var boms = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            var materials = session.CreateSQLQuery(QueryPail)
                .SetResultTransformer(Transformers.AliasToBean<ReportMaterial>())
                .SetString(0, POID)
                .SetInt32(1, pail)
                .List<ReportMaterial>();

            boms.ForEach(item => {
                var material = materials.First(p => p.Item == item.Item);
                material.BrutQuantity = item.ItemQty / boms.Count;
            });

            return Task.FromResult(materials);
        }
    }
}
