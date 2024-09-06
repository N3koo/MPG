using DataEntity.Model.Input;
using DataEntity.Model.Output;
using System.Collections.Generic;
using System.Linq;

namespace MpgWebService.Presentation.Response.Mpg {

    public record PailDto {

        public string POID { init; get; }
        public string PailNumber { init; get; }
        public decimal PailWeight { init; get; }
        public decimal MixingTime { init; get; }
        public bool LastPail { init; get; }
        public bool QC { init; get; }

        public static PailDto FromPailStatus(ProductionOrderPailStatus pail, ProductionOrder order, List<ProductionOrderTechDetail> details) => new() {
            POID = pail.POID,
            PailNumber = pail.PailNumber,
            PailWeight = pail.GrossWeight,
            LastPail = pail.PailNumber == order.PlannedQtyBUC.ToString(),
            MixingTime = details.First(p => p.OP_DESCR == "MIXARE_1").Working_Time,
            QC = pail.QC
        };
    }
}