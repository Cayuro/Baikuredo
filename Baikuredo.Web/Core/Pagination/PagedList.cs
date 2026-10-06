using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core.Extensions;

namespace Baikuredo.Web.Core.Pagination
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalCount { get; set; }

        public PagedList()
        {
            
        }

        public PagedList(List<T> items, int count, int page, int recordsPerPage)
        {
            TotalCount = count;
            CurrentPage = page;
            RecordsPerPage = recordsPerPage;
            TotalPages = (int)Math.Ceiling(count / (double)recordsPerPage);
            AddRange(items);
        }

        public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> queryable, PaginationRequest request)
        {
            int count = await queryable.CountAsync();

            List<T> items = await queryable.PaginateAsync<T>(request)
                                           .ToListAsync();

            return new PagedList<T>(items, count, request.Page, request.RecordsPerPage);
        }
    }
}
