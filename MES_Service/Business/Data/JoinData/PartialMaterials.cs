using DataEntity.Model.Input;
using DataEntity.Model.Output;
using System.Collections.Generic;

namespace MpgWebService.Business.Data.JoinData {

    public record PartialMaterials {

        public ProductionOrder Order { init; get; }
        public List<ProductionOrderPailStatus> Pails { init; get; }
        public List<ProductionOrderBom> Materials { init; get; }
        public string Position { init; get; }

    }
}
