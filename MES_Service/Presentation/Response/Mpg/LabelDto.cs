using DataEntity.Model.Input;
using DataEntity.Model.Output;
using System;

namespace MpgWebService.Presentation.Response.Mpg {

    public record LabelDto {

        public string UN { set; get; } = "UN 1263, CLASA 3";

        public string MaterialID { set; get; }
        public string Description { set; get; }
        public string KoberLot { set; get; }
        public string Standard { set; get; }
        public string Volume { set; get; }
        public string Color { set; get; }
        public string Type { set; get; }
        public string EAN { set; get; }

        public decimal? GrossWeight { set; get; }
        public decimal PlannedQty { set; get; }
        public decimal? UpperLimit { set; get; }
        public decimal? LowerLimit { set; get; }
        public decimal? NetWeight { set; get; }

        public DateTime StartDate { set; get; }
        public DateTime Validity { set; get; }

        public Classification ProductOrigin { set; get; }
        public Classification Diluent { set; get; }
        public Classification Temperature { set; get; }
        public Classification Hardener { set; get; }
        public Classification Winter { set; get; }

        public RiskPhrase RiskPhrases { set; get; }

        public void SetProductionDetails(ProductionOrder order) {
            MaterialID = order.MaterialID;
            PlannedQty = order.PlannedQtyBUC;
            KoberLot = order.KoberLot;
        }

        public void SetMaterialDetails(MaterialData material, ProductionOrderPailStatus pail) {
            var months = Convert.ToInt32(material.ShelfLife);

            Description = material.Description;
            Type = material.Type;
            NetWeight = material.NetWeight;
            GrossWeight = material.GrossWeight;
            Volume = material.Volume;
            Standard = material.Standard;
            EAN = material.EAN;

            StartDate = pail.StartDate;
            Validity = StartDate.AddMonths(months);
        }

        public void SetLimits(ProductionOrderLotDetail details) {
            UpperLimit = details.UpperLimit;
            LowerLimit = details.LowerLimit;
        }

    }
}
