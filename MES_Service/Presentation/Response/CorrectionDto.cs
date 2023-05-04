using DataEntity.Model.Input;

namespace MpgWebService.Presentation.Response {
    public record CorrectionDto {
        public string POID { set; get; }
        public string MaterialID { set; get; }
        public int PailNumber { set; get; }
        public int CorrectionID { set; get; }
        public string RawMaterialID { set; get; }
        public decimal ItemQuantity { set; get; }
        public string ItemUOM { set; get; }

        public static CorrectionDto FromCorrection(Correction correction) => new() {
            POID = correction.POID,
            MaterialID = correction.MaterialID,
            PailNumber = correction.PailNumber,
            RawMaterialID = correction.RawMaterialID,
            ItemQuantity = correction.ItemQuantity,
            ItemUOM = correction.ItemUOM
        };
    }
}
