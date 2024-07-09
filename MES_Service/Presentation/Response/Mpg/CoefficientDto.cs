using DataEntity.Model.Input;

namespace MpgWebService.Presentation.Response.Mpg {
    public record CoefficientDto {

        public string MPGHead { init; get; }
        public string MaterialID { init; get; }
        public decimal RecomQuantity { init; get; }
        public decimal MaxQuantity { init; get; }
        public decimal MPGOffset { init; get; }
        public decimal Trcoef { init; get; }
        public int NrDropsStep { init; get; }

        public static CoefficientDto FromStockVessel(StockVessel vessel) => new() {
            MPGHead = vessel.MpgHead,
            MaterialID = vessel.MaterialID,
            MPGOffset = vessel.MPGOffset,
            Trcoef = vessel.Trcoef,
            MaxQuantity = vessel.MaxQuantity,
            RecomQuantity = vessel.RecomQuantity
        };
    }
}
