using DataEntity.Model.Output;

namespace MpgWebService.Presentation.Response {

    public record PailDto {

        public string POID { init; get; }
        public string PailNumber { init; get; }
        public decimal NetWeight { init; get; }
        public decimal GrossWeight { init; get; }
        public string PailStatus { init; get; }
        public bool QC { init; get; }

        public static PailDto FromPailStatus(ProductionOrderPailStatus pail) => new() {
            POID = pail.POID,
            PailNumber = pail.PailNumber,
            NetWeight = pail.NetWeight,
            GrossWeight = pail.GrossWeight,
            PailStatus = pail.PailStatus,
            QC = pail.QC
        };
    }
}
