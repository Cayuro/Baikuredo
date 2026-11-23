using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Catalog;
using Baikuredo.Web.DTOs.Interests;
using Baikuredo.Web.DTOs.Publications;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class CatalogService : ICatalogService
    {
        private readonly DataContext _context;

        public CatalogService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<PaginationResponse<CatalogItemDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<MotorcyclePublication> query = _context.MotorcyclePublications
                    .AsNoTracking()
                    .Where(p => p.Status == PublicationStatus.Available);

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.Trim().ToLower();
                    query = query.Where(p => p.Motorcycle!.MotorcycleModel!.Brand!.Name.ToLower().Contains(filter)
                                          || p.Motorcycle!.MotorcycleModel!.Name.ToLower().Contains(filter)
                                          || p.Motorcycle!.Category!.Name.ToLower().Contains(filter)
                                          || p.Motorcycle!.Color.ToLower().Contains(filter)
                                          || p.Motorcycle!.Year.ToString().Contains(filter)
                                          || (p.Description != null && p.Description.ToLower().Contains(filter)));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(p => new CatalogItemDTO
                    {
                        Id = p.Id,
                        BrandName = p.Motorcycle!.MotorcycleModel!.Brand!.Name,
                        ModelName = p.Motorcycle!.MotorcycleModel!.Name,
                        CategoryName = p.Motorcycle!.Category!.Name,
                        Year = p.Motorcycle!.Year,
                        Displacement = p.Motorcycle!.Displacement,
                        Color = p.Motorcycle!.Color,
                        SalePrice = p.SalePrice,
                        Mileage = p.Mileage,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        MainPhotoUrl = p.Photos!
                            .OrderByDescending(f => f.IsMain)
                            .ThenBy(f => f.SortOrder)
                            .Select(f => f.Url)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var pagedList = new PagedList<CatalogItemDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<CatalogItemDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<CatalogItemDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<CatalogItemDTO>>.Failure(ex);
            }
        }

        public async Task<Response<CatalogDetailDTO>> GetDetailAsync(Guid id)
        {
            try
            {
                CatalogDetailDTO? dto = await _context.MotorcyclePublications
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.Status == PublicationStatus.Available)
                    .Select(p => new CatalogDetailDTO
                    {
                        Id = p.Id,
                        BrandName = p.Motorcycle!.MotorcycleModel!.Brand!.Name,
                        ModelName = p.Motorcycle!.MotorcycleModel!.Name,
                        CategoryName = p.Motorcycle!.Category!.Name,
                        Year = p.Motorcycle!.Year,
                        Displacement = p.Motorcycle!.Displacement,
                        Color = p.Motorcycle!.Color,
                        SalePrice = p.SalePrice,
                        Mileage = p.Mileage,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt,
                        Photos = p.Photos!
                            .OrderByDescending(f => f.IsMain)
                            .ThenBy(f => f.SortOrder)
                            .Select(f => new PhotoDTO
                            {
                                Id = f.Id,
                                Url = f.Url,
                                IsMain = f.IsMain,
                                SortOrder = f.SortOrder
                            })
                            .ToList(),
                        Interest = new CreateInterestDTO
                        {
                            MotorcyclePublicationId = p.Id
                        }
                    })
                    .FirstOrDefaultAsync();

                if (dto is null)
                {
                    return Response<CatalogDetailDTO>.Failure("La publicación solicitada no existe o ya no está disponible.");
                }

                return Response<CatalogDetailDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<CatalogDetailDTO>.Failure(ex);
            }
        }
    }
}
