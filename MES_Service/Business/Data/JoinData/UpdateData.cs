using DataEntity.Model.Input;
using System.Collections.Generic;

namespace MpgWebService.Business.Data.JoinData {

    public record UpdateData {

        public List<AlternativeName> Names { init; get; }
        public List<MaterialData> Materials { init; get; }
        public List<Classification> Classifications { init; get; }

        public UpdateData(List<AlternativeName> names, List<MaterialData> materials, List<Classification> classifications) {
            Names = names;
            Materials = materials;
            Classifications = classifications;
        }
    }
}
