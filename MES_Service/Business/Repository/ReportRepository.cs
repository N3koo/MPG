using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using NHibernate.Linq;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MpgWebService.Repository
{

    public class ReportRepository : IReportRepository {

        private readonly string QueryCommand = "SELECT pc.item, md.Description, SUM(pc.itemQty) AS NetQuantity, pc.ItemUom " +
               "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
               "WHERE pc.POID = ? GROUP BY pc.Item, md.Description, pc.ItemUom;";

        private readonly string QueryPail = "SELECT pc.item, md.Description, pc.itemQty AS NetQuantity, pc.ItemUom " +
                "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
                "WHERE pc.POID = ? AND pc.PailNumber = ?";

        public async Task<ServiceResponse<IList<ReportCommandDto>>> GetReport(Period period) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();
                var result = await session.Query<ProductionOrder>()
                    .Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToListAsync();

                if (result.Count == 0) {
                    return ServiceResponse<IList<ReportCommandDto>>.NotFound("Nu exista inregistrari in perioada selectat");
                }

                var dtos = new List<ReportCommandDto>();
                result.ForEach(item => {
                    dtos.Add(item.AsReportDto());

                    var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList();
                    dtos.AddRange(details.Select(p => p.AsReportDto(item)));
                });

                return ServiceResponse<IList<ReportCommandDto>>.Ok(dtos);
            } catch (Exception ex) {
                return ServiceResponse<IList<ReportCommandDto>>.CreateErrorMpg(ex.Message);
            }

        }

        public async Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var boms = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
                var materials = await session.CreateSQLQuery(QueryCommand)
                    .SetResultTransformer(Transformers.AliasToBean<ReportMaterialDto>())
                    .SetString(0, POID).ListAsync<ReportMaterialDto>();

                boms.ForEach(item => {
                    var material = materials.FirstOrDefault(p => p.Item == item.Item);
                    if (material != null)
                        material.BrutQuantity = item.ItemQty;
                });

                return ServiceResponse<IList<ReportMaterialDto>>.Ok(materials);
            } catch (Exception ex) {
                return ServiceResponse<IList<ReportMaterialDto>>.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForPail(string POID, int pail) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var boms = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
                var materials = await session.CreateSQLQuery(QueryPail)
                    .SetResultTransformer(Transformers.AliasToBean<ReportMaterialDto>())
                    .SetString(0, POID)
                    .SetInt32(1, pail)
                    .ListAsync<ReportMaterialDto>();

                boms.ForEach(item => {
                    var material = materials.FirstOrDefault(p => p.Item == item.Item);
                    if (material != null)
                        material.BrutQuantity = item.ItemQty / boms.Count;
                });

                return ServiceResponse<IList<ReportMaterialDto>>.Ok(materials);
            } catch (Exception ex) {
                return ServiceResponse<IList<ReportMaterialDto>>.CreateErrorMpg(ex.Message);
            }
        }
    }
}
