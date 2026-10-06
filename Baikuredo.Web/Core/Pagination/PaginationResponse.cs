namespace Baikuredo.Web.Core.Pagination
{
    public class PaginationResponse<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public string? Filter { get; set; }
        public int TotalCount { get; set; }
        public PagedList<T> List { get; set; } = new();

        public int VisiblePages { get; set; } = 5;
        public List<int> Pages
        {
            get
            {
                List<int> pages = new List<int>();

                int half = (VisiblePages / 2);
                int start = CurrentPage - half + 1 - (VisiblePages % 2);
                int end = CurrentPage + half;

                int vPages = VisiblePages;

                if (vPages > TotalPages)
                {
                    vPages = TotalPages;
                }

                if (start <= 0)
                {
                    start = 1;
                    end = vPages;
                }

                if (end > TotalPages)
                {
                    start = TotalPages - vPages + 1;
                    end = TotalPages;
                }

                int itPage = start;

                while (itPage <= end)
                {
                    pages.Add(itPage);
                    itPage += 1;
                }

                return pages;
            }
        }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
