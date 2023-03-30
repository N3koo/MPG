using MpgWebService.Data.Filter;

using System;

namespace MpgWebService.Presentation.Request {

    public class Period {
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public int PageSize { set; get; }
        public int PageNumber { set; get; }
    }
}
