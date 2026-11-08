using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Publications;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class PublicationsService : IPublicationsService
    {
        private const string TABLE_NAME = "MotorcyclePublications";
        private const long MAX_PHOTO_SIZE = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] ALLOWED_EXTENSIONS = [".jpg", ".jpeg", ".png", ".webp"];

        private readonly DataContext _context;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly IWebHostEnvironment _environment;

        public PublicationsService(DataContext context, IAuditService auditService, ICurrentUserHelper currentUserHelper, IWebHostEnvironment environment)
        {
            _context = context;
            _auditService = auditService;
            _currentUserHelper = currentUserHelper;
            _environment = environment;
        }

        public async Task<Response<PaginationResponse<PublicationDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<MotorcyclePublication> query = _context.MotorcyclePublications.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(p => p.Motorcycle!.Plate.ToLower().Contains(filter)
                                          || p.Motorcycle!.MotorcycleModel!.Name.ToLower().Contains(filter)
                                          || p.Motorcycle!.MotorcycleModel!.Brand!.Name.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(p => new PublicationDTO
                    {
                        Id = p.Id,
                        MotorcycleId = p.MotorcycleId,
                        Plate = p.Motorcycle!.Plate,
                        BrandName = p.Motorcycle!.MotorcycleModel!.Brand!.Name,
                        ModelName = p.Motorcycle!.MotorcycleModel!.Name,
                        SalePrice = p.SalePrice,
                        Mileage = p.Mileage,
                        Description = p.Description,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt,
                        ClosedAt = p.ClosedAt,
                        UserEmail = p.User!.Email!,
                        MainPhotoUrl = p.Photos!.Where(f => f.IsMain).Select(f => f.Url).FirstOrDefault()
                    })
                    .ToListAsync();

                var pagedList = new PagedList<PublicationDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<PublicationDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<PublicationDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<PublicationDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UpdatePublicationDTO>> GetOneAsync(Guid id)
        {
            try
            {
                UpdatePublicationDTO? dto = await _context.MotorcyclePublications.AsNoTracking()
                    .Where(p => p.Id == id)
                    .Select(p => new UpdatePublicationDTO
                    {
                        Id = p.Id,
                        MotorcycleText = p.Motorcycle!.Plate + " - " + p.Motorcycle!.MotorcycleModel!.Brand!.Name + " " + p.Motorcycle!.MotorcycleModel!.Name,
                        SalePrice = p.SalePrice,
                        Mileage = p.Mileage,
                        Description = p.Description,
                        Status = p.Status
                    })
                    .FirstOrDefaultAsync();

                if (dto is null)
                {
                    return Response<UpdatePublicationDTO>.Failure($"No se encontró la publicación con id: {id}");
                }

                return Response<UpdatePublicationDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdatePublicationDTO>.Failure(ex);
            }
        }

        public async Task<Response<PublicationDetailDTO>> GetDetailAsync(Guid id)
        {
            try
            {
                PublicationDetailDTO? dto = await _context.MotorcyclePublications.AsNoTracking()
                    .Where(p => p.Id == id)
                    .Select(p => new PublicationDetailDTO
                    {
                        Publication = new PublicationDTO
                        {
                            Id = p.Id,
                            MotorcycleId = p.MotorcycleId,
                            Plate = p.Motorcycle!.Plate,
                            BrandName = p.Motorcycle!.MotorcycleModel!.Brand!.Name,
                            ModelName = p.Motorcycle!.MotorcycleModel!.Name,
                            SalePrice = p.SalePrice,
                            Mileage = p.Mileage,
                            Description = p.Description,
                            Status = p.Status,
                            CreatedAt = p.CreatedAt,
                            ClosedAt = p.ClosedAt,
                            UserEmail = p.User!.Email!
                        },
                        Year = p.Motorcycle!.Year,
                        Displacement = p.Motorcycle!.Displacement,
                        Color = p.Motorcycle!.Color,
                        CategoryName = p.Motorcycle!.Category!.Name,
                        OwnerName = p.Motorcycle!.Owner!.FirstName + " " + p.Motorcycle!.Owner!.LastName,
                        Photos = p.Photos!.OrderBy(f => f.SortOrder)
                                          .Select(f => new PhotoDTO { Id = f.Id, Url = f.Url, IsMain = f.IsMain, SortOrder = f.SortOrder })
                                          .ToList(),
                        PriceHistories = p.PriceHistories!.OrderByDescending(h => h.ChangedAt)
                                                          .Select(h => new PriceHistoryDTO { PreviousPrice = h.PreviousPrice, NewPrice = h.NewPrice, ChangedAt = h.ChangedAt, UserEmail = h.User!.Email! })
                                                          .ToList(),
                        Reservations = p.Reservations!.OrderByDescending(r => r.ReservedAt)
                                                      .Select(r => new PublicationReservationDTO { CustomerName = r.CustomerName, CustomerPhone = r.CustomerPhone, DepositAmount = r.DepositAmount, ReservedAt = r.ReservedAt, ExpiresAt = r.ExpiresAt, Status = r.Status })
                                                      .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (dto is null)
                {
                    return Response<PublicationDetailDTO>.Failure($"No se encontró la publicación con id: {id}");
                }

                return Response<PublicationDetailDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<PublicationDetailDTO>.Failure(ex);
            }
        }

        public async Task<Response<Guid>> CreateAsync(CreatePublicationDTO dto)
        {
            try
            {
                Guid motorcycleId = dto.MotorcycleId!.Value;

                Motorcycle? motorcycle = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Id == motorcycleId);
                if (motorcycle is null)
                {
                    return Response<Guid>.Failure("La moto seleccionada no existe.");
                }

                bool hasActivePublication = await _context.MotorcyclePublications
                    .AnyAsync(p => p.MotorcycleId == motorcycleId && (p.Status == PublicationStatus.Available || p.Status == PublicationStatus.Reserved));
                if (hasActivePublication)
                {
                    return Response<Guid>.Failure($"La moto {motorcycle.Plate} ya tiene una publicación disponible o reservada.");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                var entity = new MotorcyclePublication
                {
                    Id = Guid.CreateVersion7(),
                    MotorcycleId = motorcycleId,
                    UserId = userId,
                    SalePrice = dto.SalePrice!.Value,
                    Mileage = dto.Mileage!.Value,
                    Description = dto.Description?.Trim(),
                    Status = PublicationStatus.Available,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.MotorcyclePublications.AddAsync(entity);
                await _auditService.AddLogAsync(userId, "CREAR", TABLE_NAME, entity.Id, $"Publicación de la moto {motorcycle.Plate} por {entity.SalePrice:N0}");
                await _context.SaveChangesAsync();

                return Response<Guid>.Success(entity.Id, "Publicación creada exitosamente. Ahora puede agregarle fotos.");
            }
            catch (Exception ex)
            {
                return Response<Guid>.Failure(ex);
            }
        }

        public async Task<Response<object>> UpdateAsync(UpdatePublicationDTO dto)
        {
            try
            {
                MotorcyclePublication? publication = await _context.MotorcyclePublications.FirstOrDefaultAsync(p => p.Id == dto.Id);
                if (publication is null)
                {
                    return Response<object>.Failure("La publicación no existe.");
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();
                decimal newPrice = dto.SalePrice!.Value;

                // 1. Si cambió el precio, se guarda en el historial
                if (publication.SalePrice != newPrice)
                {
                    await _context.PriceHistories.AddAsync(new PriceHistory
                    {
                        Id = Guid.CreateVersion7(),
                        MotorcyclePublicationId = publication.Id,
                        PreviousPrice = publication.SalePrice,
                        NewPrice = newPrice,
                        UserId = userId,
                        ChangedAt = DateTime.UtcNow
                    });

                    await _auditService.AddLogAsync(userId, "MODIFICAR_PRECIO", TABLE_NAME, publication.Id, $"De {publication.SalePrice:N0} a {newPrice:N0}");

                    publication.SalePrice = newPrice;
                }

                // 2. Si cambió el estado, se registra y se actualiza la fecha de cierre
                if (publication.Status != dto.Status)
                {
                    string action = dto.Status switch
                    {
                        PublicationStatus.Sold => "VENDER",
                        PublicationStatus.Cancelled => "CANCELAR",
                        _ => "CAMBIAR_ESTADO"
                    };

                    await _auditService.AddLogAsync(userId, action, TABLE_NAME, publication.Id, $"De {publication.Status.ToSpanish()} a {dto.Status.ToSpanish()}");

                    publication.Status = dto.Status;
                    publication.ClosedAt = dto.Status == PublicationStatus.Sold || dto.Status == PublicationStatus.Cancelled
                        ? DateTime.UtcNow
                        : null;
                }

                publication.Mileage = dto.Mileage!.Value;
                publication.Description = dto.Description?.Trim();

                _context.MotorcyclePublications.Update(publication);
                await _context.SaveChangesAsync();

                return Response<object>.Success("Publicación actualizada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(Guid id)
        {
            try
            {
                MotorcyclePublication? publication = await _context.MotorcyclePublications.Include(p => p.Photos)
                                                                                         .FirstOrDefaultAsync(p => p.Id == id);
                if (publication is null)
                {
                    return Response<object>.Failure("Publicación no encontrada.");
                }

                List<string> photoUrls = publication.Photos?.Select(f => f.Url).ToList() ?? new List<string>();

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();

                // Las fotos, el historial y las reservas se borran solos (Cascade)
                _context.MotorcyclePublications.Remove(publication);
                await _auditService.AddLogAsync(userId, "ELIMINAR", TABLE_NAME, publication.Id);
                await _context.SaveChangesAsync();

                // Después de borrar en la base de datos, se borran los archivos de las fotos
                foreach (string url in photoUrls)
                {
                    DeletePhotoFile(url);
                }

                return Response<object>.Success("Publicación eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<Guid>> AddPhotoAsync(Guid publicationId, IFormFile file)
        {
            try
            {
                if (file is null || file.Length == 0)
                {
                    return Response<Guid>.Failure("Debe seleccionar una imagen.");
                }

                if (file.Length > MAX_PHOTO_SIZE)
                {
                    return Response<Guid>.Failure("La imagen no puede pesar más de 5 MB.");
                }

                string extension = Path.GetExtension(file.FileName).ToLower();
                if (!ALLOWED_EXTENSIONS.Contains(extension))
                {
                    return Response<Guid>.Failure("Solo se permiten imágenes .jpg, .jpeg, .png o .webp.");
                }

                bool publicationExists = await _context.MotorcyclePublications.AnyAsync(p => p.Id == publicationId);
                if (!publicationExists)
                {
                    return Response<Guid>.Failure("La publicación no existe.");
                }

                // Se guarda en wwwroot/uploads/publications/{idPublicacion}/{nombreAleatorio}.jpg
                string folder = Path.Combine(_environment.WebRootPath, "uploads", "publications", publicationId.ToString());
                Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(folder, fileName);

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                int photosCount = await _context.MotorcyclePhotos.CountAsync(f => f.MotorcyclePublicationId == publicationId);

                var photo = new MotorcyclePhoto
                {
                    Id = Guid.CreateVersion7(),
                    MotorcyclePublicationId = publicationId,
                    Url = $"/uploads/publications/{publicationId}/{fileName}",
                    IsMain = photosCount == 0, // la primera foto queda como portada
                    SortOrder = photosCount + 1
                };

                await _context.MotorcyclePhotos.AddAsync(photo);
                await _context.SaveChangesAsync();

                return Response<Guid>.Success(publicationId, "Foto agregada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<Guid>.Failure(ex);
            }
        }

        public async Task<Response<Guid>> DeletePhotoAsync(Guid photoId)
        {
            try
            {
                MotorcyclePhoto? photo = await _context.MotorcyclePhotos.FirstOrDefaultAsync(f => f.Id == photoId);
                if (photo is null)
                {
                    return Response<Guid>.Failure("La foto no existe.");
                }

                Guid publicationId = photo.MotorcyclePublicationId;
                bool wasMain = photo.IsMain;
                string url = photo.Url;

                _context.MotorcyclePhotos.Remove(photo);
                await _context.SaveChangesAsync();

                // Si era la portada, la siguiente foto pasa a ser la portada
                if (wasMain)
                {
                    MotorcyclePhoto? next = await _context.MotorcyclePhotos.Where(f => f.MotorcyclePublicationId == publicationId)
                                                                          .OrderBy(f => f.SortOrder)
                                                                          .FirstOrDefaultAsync();
                    if (next is not null)
                    {
                        next.IsMain = true;
                        await _context.SaveChangesAsync();
                    }
                }

                DeletePhotoFile(url);

                return Response<Guid>.Success(publicationId, "Foto eliminada con éxito.");
            }
            catch (Exception ex)
            {
                return Response<Guid>.Failure(ex);
            }
        }

        public async Task<Response<Guid>> SetMainPhotoAsync(Guid photoId)
        {
            try
            {
                MotorcyclePhoto? photo = await _context.MotorcyclePhotos.FirstOrDefaultAsync(f => f.Id == photoId);
                if (photo is null)
                {
                    return Response<Guid>.Failure("La foto no existe.");
                }

                List<MotorcyclePhoto> photos = await _context.MotorcyclePhotos.Where(f => f.MotorcyclePublicationId == photo.MotorcyclePublicationId)
                                                                             .ToListAsync();

                foreach (MotorcyclePhoto item in photos)
                {
                    item.IsMain = item.Id == photoId;
                }

                await _context.SaveChangesAsync();

                return Response<Guid>.Success(photo.MotorcyclePublicationId, "Portada actualizada.");
            }
            catch (Exception ex)
            {
                return Response<Guid>.Failure(ex);
            }
        }

        // Borra el archivo físico de una foto (si existe)
        private void DeletePhotoFile(string url)
        {
            string relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.Combine(_environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
