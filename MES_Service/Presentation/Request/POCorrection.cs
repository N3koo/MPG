using DataEntity.Model.Output;

using System;

namespace MpgWebService.Presentation.Request {
    public record POCorrection {
        public string POID { set; get; }
        public string MaterialID { set; get; }
        public string PailNumber { set; get; }
        public int CorrectionID { set; get; }
        public string Item_1 { set; get; }
        public double ItemQty_1 { set; get; }
        public string ItemUom_1 { set; get; }
        public string Item_2 { set; get; }
        public double ItemQty_2 { set; get; }
        public string ItemUom_2 { set; get; }
        public string Item_3 { set; get; }
        public double ItemQty_3 { set; get; }
        public string ItemUom_3 { set; get; }
        public string Item_4 { set; get; }
        public double ItemQty_4 { set; get; }
        public string ItemUom_4 { set; get; }
        public string Item_5 { set; get; }
        public double ItemQty_5 { set; get; }
        public string ItemUom_5 { set; get; }
        public string Item_6 { set; get; }
        public double ItemQty_6 { set; get; }
        public string ItemUom_6 { set; get; }
        public string Item_7 { set; get; }
        public double ItemQty_7 { set; get; }
        public string ItemUom_7 { set; get; }
        public string Item_8 { set; get; }
        public double ItemQty_8 { set; get; }
        public string ItemUom_8 { set; get; }
        public string Item_9 { set; get; }
        public double ItemQty_9 { set; get; }
        public string ItemUom_9 { set; get; }
        public string Item_10 { set; get; }
        public double ItemQty_10 { set; get; }
        public string ItemUom_10 { set; get; }

        public static ProductionOrderCorection CreatePOCorrection(POCorrection dto) => new() {
            CreationDate = DateTime.Now,
            POID = dto.POID,
            PailNumber = dto.PailNumber,
            CorrectionID = dto.CorrectionID,
            Item_1 = dto.Item_1,
            ItemQty_1 = dto.ItemQty_1,
            ItemUom_1 = dto.ItemUom_1,
            Item_2 = dto.Item_2,
            ItemQty_2 = dto.ItemQty_2,
            ItemUom_2 = dto.ItemUom_2,
            Item_3 = dto.Item_3,
            ItemQty_3 = dto.ItemQty_3,
            ItemUom_3 = dto.ItemUom_3,
            Item_4 = dto.Item_4,
            ItemQty_4 = dto.ItemQty_4,
            ItemUom_4 = dto.ItemUom_4,
            Item_5 = dto.Item_5,
            ItemQty_5 = dto.ItemQty_5,
            ItemUom_5 = dto.ItemUom_5,
            Item_6 = dto.Item_6,
            ItemQty_6 = dto.ItemQty_6,
            ItemUom_6 = dto.ItemUom_6,
            Item_7 = dto.Item_7,
            ItemQty_7 = dto.ItemQty_7,
            ItemUom_7 = dto.ItemUom_7,
            Item_8 = dto.Item_8,
            ItemQty_8 = dto.ItemQty_8,
            ItemUom_8 = dto.ItemUom_8,
            Item_9 = dto.Item_9,
            ItemQty_9 = dto.ItemQty_9,
            ItemUom_9 = dto.ItemUom_9,
            Item_10 = dto.Item_10,
            ItemQty_10 = dto.ItemQty_10,
            ItemUom_10 = dto.ItemUom_10,
            MPGStatus = 1,
            MESStatus = 0,
            MPGRowUpdated = DateTime.Now
        };
    }
}
