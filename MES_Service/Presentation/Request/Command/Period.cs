using System;

namespace MpgWebService.Presentation.Request.Command {

    public class Period {

        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public int PageSize { set; get; }
        public int PageNumber { set; get; }

    }
}
