using MpgWebService.Business.Data.DTO;
using MpgWebService.Properties;

using DataEntity.Model.Input;

using SAPServices;

using System.Collections.Generic;
using System.Globalization;
using System;

namespace MpgWebService.Data.Extension {

    public static class ProductionOrderExtension {

        public static ProductionOrderDto AsDto(this ProductionOrder po) {
            return new ProductionOrderDto {
                PODescription = po.PODescription,
                POID = po.POID,
                MaterialID = po.MaterialID,
                PlantID = po.PlantID,
                Status = po.Status,
                PlannedQtyBUC = po.PlannedQtyBUC,
                PlannedQtyBUCUom = po.PlannedQtyBUCUom,
                KoberLot = po.KoberLot,
                ProfitCenter = po.ProfitCenter,
                Priority = po.Priority,
                PlannedStartDate = po.PlannedStartDate,
                PlannedStartHour = po.PlannedStartHour,
                PlannedEndDate = po.PlannedEndDate,
                PlannedEndHour = po.PlannedEndHour
            };
        }

        public static Z_MPGPREDARE CreatePredare(this ProductionOrder po, int pailsNumber, string position) {
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var receipeHeader = new ZGOODSRECEIPTHEADER {
                POID = po.POID,
                GOODSRECEIPTTYPE = Resources.RECEIPT_TYPE,
                POSTINGDATE = date,
                DOCDATE = date,
                PLANT = po.PlantID,
                PROFITCENTER = po.ProfitCenter,
                HEADERTEXT = $"{po.POID}-{po.KoberLot}"
            };

            var receipeItem = new ZGOODSRECEIPTITEMS {
                POID = po.POID,
                GOODSRECEIPTTYPE = Resources.RECEIPT_TYPE,
                ITEMPOSITION = position,
                MATERIALID = po.MaterialID,
                QUANTITYPRODUCED = pailsNumber,
                UOM = po.PlannedQtyBUCUom,
                STORAGELOC = po.StorageLoc,
                KOBERLOT = po.KoberLot
            };

            return new Z_MPGPREDARE {
                GOODSRECEIPTHEADER = new ZGOODSRECEIPTHEADER[1] { receipeHeader },
                GOODSRECEIPTITEMS = new ZGOODSRECEIPTITEMS[1] { receipeItem }
            };
        }

        public static Z_MPGCONSUM CreateConsumption(this ProductionOrder po, List<ProductionOrderBom> list) {
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<ZCONSUMPTIONITEMS> items = new();

            var headerBC = new ZCONSUMPTIONHEADER {
                POID = po.POID,
                CONSUMPTIONTYPE = Resources.CONSUMPTION_TYPE,
                POSTINGDATE = date,
                DOCDATE = date,
                MATERIALID = po.MaterialID,
                PLANT = po.PlantID,
                PROFITCENTER = po.ProfitCenter,
                HEADERTEXT = $"{po.POID}-{po.KoberLot}",
                KOBERLOT = po.KoberLot,
                REZERVATIONNUMBER = po.RezervationNumber
            };

            list.ForEach(item => {
                items.Add(new ZCONSUMPTIONITEMS {
                    POID = po.POID,
                    CONSUMPTIONTYPE = Resources.CONSUMPTION_TYPE,
                    ITEMPOSITION = item.ItemPosition,
                    ROWMATERIALID = item.ItemStorageLoc,
                    QUANTITY = item.ItemQty,
                    UOM = item.ItemQtyUOM,
                    LOT = item.ItemProposedLot
                });
            });

            return new Z_MPGCONSUM {
                CONSUMPTIONHEADER = new ZCONSUMPTIONHEADER[1] { headerBC },
                CONSUMPTIONITEMS = items.ToArray()
            };
        }

        public static Production AsProductionDto(this ProductionOrder order) {
            return new Production {
                POID = order.POID,
                POID_ID = "-1",
                Name = order.PODescription,
                Quantity = order.PlannedQtyBUC,
                Status = order.Status,
                Unit = order.PlannedQtyBUCUom,
                KoberLot = order.KoberLot,
                MaterialID = order.MaterialID
            };
        }

        public static ReportCommand AsReportDto(this ProductionOrder item) {
            return new ReportCommand {
                POID = item.POID,
                POID_ID = "-1",
                Product = item.MaterialID,
                KoberLot = item.KoberLot,
                Name = item.PODescription,
                Quantity = item.PlannedQtyBUC,
                UOM = item.PlannedQtyBUCUom,
                StartDate = item.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = item.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ExecuteDate = null,
                Status = item.Status
            };
        }

    }
}
