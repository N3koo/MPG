namespace MpgWebService.Presentation.Request {

    public class StartCommand {
        public string POID { set; get; }
        public int? Priority { set; get; }
        public bool[] QC { set; get; }
    }
}
