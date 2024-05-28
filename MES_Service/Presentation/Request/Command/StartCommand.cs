namespace MpgWebService.Presentation.Request.Command {

    public class StartCommand {

        public string POID { set; get; }
        public int? Priority { set; get; }
        public bool[] QC { set; get; }

    }
}
