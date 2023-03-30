namespace MpgWebService.Data.Filter {

    public class PaginationFilter {

        public int PageNumber { set; get; }
        public int PageSize { set; get; }

        public PaginationFilter() {
            PageNumber = 1;
            PageSize = 10;
        }

        public PaginationFilter(int pageNumber, int pageSize) {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : PageSize;
        }

        public PaginationFilter(PaginationFilter filter) {
            PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            PageSize = filter.PageSize > 10 ? 10 : filter.PageSize;
        }
    }
}
