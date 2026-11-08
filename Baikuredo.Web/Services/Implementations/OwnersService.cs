using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Owners;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class OwnersService : IOwnersService
    {
        private readonly DataContext _context;

        public OwnersService(DataContext context)
        {
            _context = context;
        }

        public async Task<Response<PaginationResponse<OwnerDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<Owner> query = _context.Owners.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(o => o.FirstName.ToLower().Contains(filter)
                                          || o.LastName.ToLower().Contains(filter)
                                          || o.Document.ToLower().Contains(filter)
                                          || o.Phone.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(o => o.FirstName)
                    .ThenBy(o => o.LastName)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(o => new OwnerDTO
                    {
                        Id = o.Id,
                        DocumentType = o.DocumentType,
                        Document = o.Document,
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Phone = o.Phone,
                        Email = o.Email,
                        Address = o.Address
                    })
                    .ToListAsync();

                var pagedList = new PagedList<OwnerDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<OwnerDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<OwnerDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<OwnerDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdateOwnerDTO>> GetOneAsync(Guid id)
        {
            try
            {
                Owner? owner = await _context.Owners.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

                if (owner is null)
                {
                    return Response<UpdateOwnerDTO>.Failure($"No se encontró el propietario con id: {id}");
                }

                var dto = new UpdateOwnerDTO
                {
                    Id = owner.Id,
                    DocumentType = owner.DocumentType,
                    Document = owner.Document,
                    FirstName = owner.FirstName,
                    LastName = owner.LastName,
                    Phone = owner.Phone,
                    Email = owner.Email,
                    Address = owner.Address
                };

                return Response<UpdateOwnerDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateOwnerDTO>.Failure(ex);
            }
        }

        public async Task<Response<OwnerDTO>> CreateAsync(CreateOwnerDTO dto)
        {
            try
            {
                string documentType = dto.DocumentType.Trim();
                string document = dto.Document.Trim();

                bool exists = await _context.Owners.AnyAsync(o => o.DocumentType == documentType && o.Document == document);
                if (exists)
                {
                    return Response<OwnerDTO>.Failure($"Ya existe un propietario con documento {documentType} {document}.");
                }

                var entity = new Owner
                {
                    Id = Guid.CreateVersion7(),
                    DocumentType = documentType,
                    Document = document,
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    Phone = dto.Phone.Trim(),
                    Email = dto.Email?.Trim(),
                    Address = dto.Address?.Trim()
                };

                await _context.Owners.AddAsync(entity);
                await _context.SaveChangesAsync();

                var resultDto = new OwnerDTO
                {
                    Id = entity.Id,
                    DocumentType = entity.DocumentType,
                    Document = entity.Document,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    Address = entity.Address
                };

                return Response<OwnerDTO>.Success(resultDto, "Propietario creado exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<OwnerDTO>.Failure(ex);
            }
        }

        public async Task<Response<OwnerDTO>> UpdateAsync(UpdateOwnerDTO dto)
        {
            try
            {
                Owner? owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == dto.Id);
                if (owner is null)
                {
                    return Response<OwnerDTO>.Failure("El propietario no existe.");
                }

                string documentType = dto.DocumentType.Trim();
                string document = dto.Document.Trim();

                bool duplicateExists = await _context.Owners
                    .AnyAsync(o => o.DocumentType == documentType && o.Document == document && o.Id != dto.Id);
                if (duplicateExists)
                {
                    return Response<OwnerDTO>.Failure("Ya existe otro propietario con ese documento.");
                }

                owner.DocumentType = documentType;
                owner.Document = document;
                owner.FirstName = dto.FirstName.Trim();
                owner.LastName = dto.LastName.Trim();
                owner.Phone = dto.Phone.Trim();
                owner.Email = dto.Email?.Trim();
                owner.Address = dto.Address?.Trim();

                _context.Owners.Update(owner);
                await _context.SaveChangesAsync();

                return Response<OwnerDTO>.Success("Propietario actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<OwnerDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                Owner? owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == id);
                if (owner is null)
                {
                    return Response<object>.Failure("Propietario no encontrado.");
                }

                _context.Owners.Remove(owner);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Propietario eliminado con éxito.");
            }
            catch (DbUpdateException)
            {
                return Response<object>.Failure("No se puede eliminar el propietario porque tiene motos registradas.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }
    }
}
