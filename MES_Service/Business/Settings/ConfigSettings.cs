using MpgWebService.Business.Interface.Settings;

namespace MpgWebService.Business.Settings {

    public class ConfigSettings : ISettings {
        public string Plant {
            get => Properties.Settings.Default.Plant;
            set => Properties.Settings.Default.Plant = value;
        }

        public string Update {
            get => Properties.Settings.Default.Update;
            set => Properties.Settings.Default.Update = value;
        }

        public string Code {
            get => Properties.Settings.Default.Code;
            set => Properties.Settings.Default.Code = value;
        }

        public string Connection {
            get => Properties.Settings.Default.Connection;
            set => Properties.Settings.Default.Connection = value;
        }

        public string CMD_BLOCKED {
            get => Properties.Settings.Default.CMD_BLOCKED;
            set => Properties.Settings.Default.CMD_BLOCKED = value;
        }

        public string CMD_DONE {
            get => Properties.Settings.Default.CMD_DONE;
            set => Properties.Settings.Default.CMD_DONE = value;
        }

        public string CMD_ERROR {
            get => Properties.Settings.Default.CMD_ERROR;
            set => Properties.Settings.Default.CMD_ERROR = value;
        }

        public string CMD_QC {
            get => Properties.Settings.Default.CMD_QC;
            set => Properties.Settings.Default.CMD_QC = value;
        }

        public string CMD_SEND {
            get => Properties.Settings.Default.CMD_SEND;
            set => Properties.Settings.Default.CMD_SEND = value;
        }

        public string CMD_STARTED {
            get => Properties.Settings.Default.CMD_STARTED;
            set => Properties.Settings.Default.CMD_STARTED = value;
        }

        public string CONSUMPTION_TYPE {
            get => Properties.Settings.Default.CONSUMPTION_TYPE;
            set => Properties.Settings.Default.CONSUMPTION_TYPE = value;
        }

        public string MAXIMUM_DOSAGE_TIME {
            get => Properties.Settings.Default.MAXIMUM_DOSAGE_TIME;
            set => Properties.Settings.Default.MAXIMUM_DOSAGE_TIME = value;
        }

        public string RECEIPT_TYPE {
            get => Properties.Settings.Default.RECEIPT_TYPE;
            set => Properties.Settings.Default.RECEIPT_TYPE = value;
        }
    }
}
