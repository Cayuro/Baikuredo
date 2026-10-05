using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core.Extensions;
using Baikuredo.Web.DTOs.Categoria;

namespace Baikuredo.Web.Core.Pagination
{
    public class PagedList<T> : List<T>
    {
        private List<CategoriaDTO> items;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            RecordsPerPage = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public PagedList(List<CategoriaDTO> items)
        {
            this.items = items;
        }

        public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> queryable, PaginationRequest request)
        {
            int count = await queryable.CountAsync();
            List<T> items = await queryable.PaginateAsync(request).ToListAsync();
            return new PagedList<T>(items, count, request.Page, request.RecordsPerPage);
        }
    }
}
