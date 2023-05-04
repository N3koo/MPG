using DataEntity.Model.Output;

using System;

namespace MpgWebService.Presentation.Request {
    public record POConsumption {
        public string POID { init; get; }
        public string MaterialID { init; get; }
        public string PailNumber { init; get; }
        public string Item { init; get; }
        public decimal ItemQty { init; get; }
        public string ItemUom { init; get; }
        public string ItemLot { init; get; }
        public string ItemStorageLoc { init; get; }

        public static ProductionOrderConsumption CreateConsumption(POConsumption dto) => new() {
            CreationDate = DateTime.Now,
            POID = dto.POID,
            MaterialID = dto.MaterialID,
            PailNumber = dto.PailNumber,
            Item = dto.Item,
            ItemQty = dto.ItemQty,
            ItemUom = dto.ItemUom,
            ItemStorageLoc = dto.ItemStorageLoc,
            MPGStatus = 1,
            MESStatus = 0,
            MPGRowUpdated = DateTime.Now
        };
    }
}
