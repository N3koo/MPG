using DataEntity.Model.Output;
using DataEntity.Model.Types;
using System.Collections.Generic;

namespace MpgWebService.Business.Data.JoinData {

    public record CommandData {

        public InputData Data { init; get; }
        public List<ProductionOrderPailStatus> Pails { init; get; }

    }
}
