namespace Baikuredo.Web.Core.Pagination
{
    public class PaginationRequest
    {
        private int _page = 1;
        private int _recordsPerPage = 15;
        private const int MAX_RECORDS_PER_PAGE = 50;

        public string? Filter { get; set; }

        public int Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }

        public int RecordsPerPage
        {
            get => _recordsPerPage;
            set => _recordsPerPage = value <= MAX_RECORDS_PER_PAGE ? value : MAX_RECORDS_PER_PAGE;
        }

        public static PaginationRequest Default => new() { Page = 1, RecordsPerPage = 15 };
    }
}
