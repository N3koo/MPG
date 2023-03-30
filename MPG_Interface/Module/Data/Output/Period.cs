using System;

namespace MPG_Interface.Module.Data.Output {
    public class Period {
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public int PageSize { set; get; }
        public int PageNumber { set; get; }
    }
}
