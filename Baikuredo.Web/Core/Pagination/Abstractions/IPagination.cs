namespace Baikuredo.Web.Core.Pagination.Abstractions
{
    public interface IPagination
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public string? Filter { get; set; }
        public int TotalCount { get; set; }
        public List<int> Pages { get; }
        public bool HasPrevious { get; }
        public bool HasNext { get; }
    }
}
