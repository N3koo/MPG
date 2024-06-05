using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Business.Data.JoinData;

using DataEntity.Model.Output;
using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Linq;
using System;

using NHibernate;
using System.Threading.Tasks;
using MpgWebService.Data.Wrappers;
using MpgWebService.Presentation.Response;

namespace MpgWebService.Business.Data.Utils {

    public class Utils {

        public static List<ProductionOrderConsumption> CreateConsumption(POConsumption materials, List<ProductionOrderBom> bom) {
            var result = new List<ProductionOrderConsumption>();
            var currentDate = DateTime.Now;

            bom.ForEach(item => {
                var material = materials.Materials.First(p => p.Item == item.Item);

                result.Add(new() {
                    CreationDate = currentDate,
                    POID = materials.POID,
                    MaterialID = item.MaterialID,
                    PailNumber = materials.PailNumber,
                    Item = item.Item,
                    ItemQty = material.ItemQty,
                    ItemUom = material.ItemUom,
                    ItemLot = item.ItemProposedLot,
                    ItemStorageLoc = item.ItemStorageLoc,
                    MPGStatus = 1,
                    MESStatus = 0,
                    ErrorMessage = null,
                    MPGRowUpdated = currentDate
                });
            });

            return result;
        }

        public static List<LotDetails> GetOperations(ISession session, string POID) {
            return session.Query<ProductionOrderLotDetail>().Where(p => p.POID == POID).OrderBy(p => p.OpNo)
                .GroupBy(p => new { p.OpNo, p.OpDescr })
                .Select(p => new LotDetails { OpDescr = p.Key.OpDescr, OPNO = p.Key.OpNo })
                .ToList();
        }

        public static bool CheckOperation(ISession session, string POID, int pailNumber, string opNo) {
            return session.Query<QualityCheck>().Any(p => p.POID == POID &&
                p.PailNumber == pailNumber && p.OpNo == opNo && p.QualityOK != 2);
        }

        public static void UpdateMaterials(ISession session, POConsumption materials) {
            var planned = session.Query<ProductionOrder>().First(p => p.POID == materials.POID).PlannedQtyBUC;

            foreach (var material in materials.Materials) {
                var bom = session.Query<ProductionOrderBom>().FirstOrDefault(p => p.POID == materials.POID && p.Item == material.Item);

                if (bom != null) {
                    bom.ItemQty += material.ItemQty * planned;
                    session.Update(bom);
                } else {
                    var lastBom = session.Query<ProductionOrderBom>().Where(p => p.POID == materials.POID).OrderByDescending(p => p.ItemPosition).First();
                    var newBom = CreateElement(bom, material);
                    session.Save(newBom);
                }
            }
        }

        private static ProductionOrderBom CreateElement(ProductionOrderBom bom, UsedMaterial material) => new() {
            POID = bom.POID,
            Item = material.Item,
            ItemPosition = (int.Parse(bom.ItemPosition) + 1).ToString(),
            BomAlternative = bom.BomAlternative,
            BomID = bom.BomID,
            ItemProposedLot = bom.ItemProposedLot,
            ItemQty = material.ItemQty,
            ItemQtyUOM = material.ItemUom, 
            ItemStorageLoc = bom.ItemStorageLoc,
            MaterialID = bom.MaterialID
        };
        
        public static ProductionOrderCorection CreateCorrection(POConsumption materials, QualityCheck quality) => new() {
            CreationDate = DateTime.Now,
            POID = materials.POID,
            MaterialID = quality.MaterialID,
            PailNumber = materials.PailNumber,
            CorrectionID = quality.ID,

            Item_1 = materials.Materials[0]?.Item,
            ItemQty_1 = (double)materials.Materials[0]?.ItemQty,
            ItemUom_1 = materials.Materials[0]?.ItemUom,

            Item_2 = materials.Materials[1]?.Item,
            ItemQty_2 = (double)materials.Materials[1]?.ItemQty,
            ItemUom_2 = materials.Materials[1]?.ItemUom,

            Item_3 = materials.Materials[2]?.Item,
            ItemQty_3 = (double)materials.Materials[2]?.ItemQty,
            ItemUom_3 = materials.Materials[2]?.ItemUom,

            Item_4 = materials.Materials[3]?.Item,
            ItemQty_4 = (double)materials.Materials[3]?.ItemQty,
            ItemUom_4 = materials.Materials[3]?.ItemUom,

            Item_5 = materials.Materials[4]?.Item,
            ItemQty_5 = (double)materials.Materials[4]?.ItemQty,
            ItemUom_5 = materials.Materials[4]?.ItemUom,

            Item_6 = materials.Materials[5]?.Item,
            ItemQty_6 = (double)materials.Materials[5]?.ItemQty,
            ItemUom_6 = materials.Materials[5]?.ItemUom,

            Item_7 = materials.Materials[6]?.Item,
            ItemQty_7 = (double)materials.Materials[6]?.ItemQty,
            ItemUom_7 = materials.Materials[6]?.ItemUom,

            Item_8 = materials.Materials[7]?.Item,
            ItemQty_8 = (double)materials.Materials[7]?.ItemQty,
            ItemUom_8 = materials.Materials[7]?.ItemUom,

            Item_9 = materials.Materials[8]?.Item,
            ItemQty_9 = (double)materials.Materials[8]?.ItemQty,
            ItemUom_9 = materials.Materials[8]?.ItemUom,

            Item_10 = materials.Materials[9]?.Item,
            ItemQty_10 = (double)materials.Materials[9]?.ItemQty,
            ItemUom_10 = materials.Materials[9]?.ItemUom,

            MPGStatus = 1,
            MESStatus = 0,
            ErrorMessage = null,
            MPGRowUpdated = DateTime.Now
        };

        public static ServiceResponse CatchError(Func<ServiceResponse> function) {
            try {
                return function();
            } catch (Exception ex) {
                return ServiceResponse.InternalError(ex);
            }
        }
    }
}
