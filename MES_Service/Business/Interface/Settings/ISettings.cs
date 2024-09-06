namespace MpgWebService.Business.Interface.Settings {

    public interface ISettings {

        public string Plant { get; set; }
        public string Update { get; set; }
        public string Code { get; set; }
        public string Connection { get; set; }
        public string CMD_BLOCKED { get; set; }
        public string CMD_DONE { get; set; }
        public string CMD_ERROR { get; set; }
        public string CMD_QC { get; set; }
        public string CMD_SEND { get; set; }
        public string CMD_STARTED { get; set; }
        public string CONSUMPTION_TYPE { get; set; }
        public string MAXIMUM_DOSAGE_TIME { get; set; }
        public string RECEIPT_TYPE { get; set; }

    }
}
