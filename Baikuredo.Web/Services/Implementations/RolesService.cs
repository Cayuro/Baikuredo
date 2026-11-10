using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Roles;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    // Reglas:
    // - El rol Admin no se puede editar ni eliminar (tiene acceso a todo y nadie debe quedarse sin él).
    // - No se puede eliminar un rol que tenga usuarios asignados.
    // - El nombre del rol no se repite.
    public class RolesService : IRolesService
    {
        private const string TABLE_NAME = "BaikuredoRoles";

        private readonly DataContext _context;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserHelper _currentUserHelper;

        public RolesService(DataContext context, IAuditService auditService, ICurrentUserHelper currentUserHelper)
        {
            _context = context;
            _auditService = auditService;
            _currentUserHelper = currentUserHelper;
        }

        public async Task<Response<PaginationResponse<RoleListDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<BaikuredoRole> query = _context.BaikuredoRoles.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(r => r.Name.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(r => r.Name)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(r => new RoleListDTO
                    {
                        Id = r.Id,
                        Name = r.Name,
                        UsersCount = _context.Users.Count(u => u.BaikuredoRoleId == r.Id),
                        PermissionsCount = r.RolePermissions!.Count(),
                        IsSuperAdmin = r.Name == Env.SUPER_ADMIN_ROLE_NAME
                    })
                    .ToListAsync();

                var pagedList = new PagedList<RoleListDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<RoleListDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<RoleListDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<RoleListDTO>>.Failure(ex);
            }
        }

        public async Task<Response<RoleFormDTO>> GetNewFormAsync()
        {
            try
            {
                var dto = new RoleFormDTO
                {
                    Permissions = await GetAllPermissionsAsync(new List<Guid>())
                };

                return Response<RoleFormDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<RoleFormDTO>.Failure(ex);
            }
        }

        public async Task<Response<RoleFormDTO>> GetOneAsync(Guid id)
        {
            try
            {
                BaikuredoRole? role = await _context.BaikuredoRoles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

                if (role is null)
                {
                    return Response<RoleFormDTO>.Failure($"No se encontró el rol con id: {id}");
                }

                if (role.Name == Env.SUPER_ADMIN_ROLE_NAME)
                {
                    return Response<RoleFormDTO>.Failure("El rol Admin no se puede editar: tiene acceso a todo el sistema.");
                }

                List<Guid> selectedIds = await _context.RolePermissions.Where(rp => rp.BaikuredoRoleId == id)
                                                                       .Select(rp => rp.PermissionId)
                                                                       .ToListAsync();

                var dto = new RoleFormDTO
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = await GetAllPermissionsAsync(selectedIds)
                };

                return Response<RoleFormDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<RoleFormDTO>.Failure(ex);
            }
        }

        public async Task<Response<object>> CreateAsync(RoleFormDTO dto)
        {
            try
            {
                string name = dto.Name.Trim();

                if (name.ToLower() == Env.SUPER_ADMIN_ROLE_NAME.ToLower())
                {
                    return Response<object>.Failure($"El nombre {Env.SUPER_ADMIN_ROLE_NAME} está reservado.");
                }

                bool exists = await _context.BaikuredoRoles.AnyAsync(r => r.Name.ToLower() == name.ToLower());
                if (exists)
                {
                    return Response<object>.Failure($"Ya existe un rol llamado {name}.");
                }

                var role = new BaikuredoRole
                {
                    Id = Guid.CreateVersion7(),
                    Name = name
                };

                await _context.BaikuredoRoles.AddAsync(role);

                List<Guid> selectedIds = await GetValidSelectedIdsAsync(dto.Permissions);
                foreach (Guid permissionId in selectedIds)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission { BaikuredoRoleId = role.Id, PermissionId = permissionId });
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();
                await _auditService.AddLogAsync(userId, "CREAR", TABLE_NAME, role.Id, $"Rol {name} con {selectedIds.Count} permisos");
                await _context.SaveChangesAsync();

                return Response<object>.Success($"Rol {name} creado con {selectedIds.Count} permisos.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<Response<object>> UpdateAsync(RoleFormDTO dto)
        {
            try
            {
                BaikuredoRole? role = await _context.BaikuredoRoles.FirstOrDefaultAsync(r => r.Id == dto.Id);
                if (role is null)
                {
                    return Response<object>.Failure("El rol no existe.");
                }

                if (role.Name == Env.SUPER_ADMIN_ROLE_NAME)
                {
                    return Response<object>.Failure("El rol Admin no se puede editar.");
                }

                string name = dto.Name.Trim();

                if (name.ToLower() == Env.SUPER_ADMIN_ROLE_NAME.ToLower())
                {
                    return Response<object>.Failure($"El nombre {Env.SUPER_ADMIN_ROLE_NAME} está reservado.");
                }

                bool duplicateExists = await _context.BaikuredoRoles.AnyAsync(r => r.Name.ToLower() == name.ToLower() && r.Id != role.Id);
                if (duplicateExists)
                {
                    return Response<object>.Failure("Ya existe otro rol con ese nombre.");
                }

                role.Name = name;

                // Se borran los permisos que tenía y se guardan los marcados ahora
                List<RolePermission> currentPermissions = await _context.RolePermissions.Where(rp => rp.BaikuredoRoleId == role.Id)
                                                                                        .ToListAsync();
                _context.RolePermissions.RemoveRange(currentPermissions);

                List<Guid> selectedIds = await GetValidSelectedIdsAsync(dto.Permissions);
                foreach (Guid permissionId in selectedIds)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission { BaikuredoRoleId = role.Id, PermissionId = permissionId });
                }

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();
                await _auditService.AddLogAsync(userId, "MODIFICAR", TABLE_NAME, role.Id, $"Rol {name} con {selectedIds.Count} permisos");
                await _context.SaveChangesAsync();

                return Response<object>.Success($"Rol {name} actualizado con {selectedIds.Count} permisos.");
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
                BaikuredoRole? role = await _context.BaikuredoRoles.FirstOrDefaultAsync(r => r.Id == id);
                if (role is null)
                {
                    return Response<object>.Failure("Rol no encontrado.");
                }

                if (role.Name == Env.SUPER_ADMIN_ROLE_NAME)
                {
                    return Response<object>.Failure("El rol Admin no se puede eliminar.");
                }

                // Importante: si se borrara un rol con usuarios, la base de datos borraría también esos usuarios
                int usersCount = await _context.Users.CountAsync(u => u.BaikuredoRoleId == id);
                if (usersCount > 0)
                {
                    return Response<object>.Failure($"No se puede eliminar el rol {role.Name} porque tiene {usersCount} usuario(s). Cámbieles el rol primero.");
                }

                // Los RolePermissions se borran solos (Cascade)
                _context.BaikuredoRoles.Remove(role);

                string userId = await _currentUserHelper.GetCurrentUserIdAsync();
                await _auditService.AddLogAsync(userId, "ELIMINAR", TABLE_NAME, role.Id, $"Rol {role.Name}");
                await _context.SaveChangesAsync();

                return Response<object>.Success($"Rol {role.Name} eliminado con éxito.");
            }
            catch (Exception ex)
            {
                return Response<object>.Failure(ex);
            }
        }

        public async Task<List<PermissionForRoleDTO>> RebuildPermissionsAsync(List<PermissionForRoleDTO> posted)
        {
            List<Guid> selectedIds = posted.Where(p => p.Selected).Select(p => p.Id).ToList();
            return await GetAllPermissionsAsync(selectedIds);
        }

        // Todos los permisos, ordenados por módulo, marcando los que vienen en selectedIds
        private async Task<List<PermissionForRoleDTO>> GetAllPermissionsAsync(List<Guid> selectedIds)
        {
            List<Permission> permissions = await _context.Permissions.AsNoTracking()
                                                                     .OrderBy(p => p.Module)
                                                                     .ThenBy(p => p.Code)
                                                                     .ToListAsync();

            return permissions.Select(p => new PermissionForRoleDTO
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description,
                Module = p.Module,
                Selected = selectedIds.Contains(p.Id)
            }).ToList();
        }

        // Solo se aceptan Ids de permisos que de verdad existen (por si alguien modifica el formulario)
        private async Task<List<Guid>> GetValidSelectedIdsAsync(List<PermissionForRoleDTO> posted)
        {
            List<Guid> selected = posted.Where(p => p.Selected).Select(p => p.Id).Distinct().ToList();

            return await _context.Permissions.Where(p => selected.Contains(p.Id))
                                             .Select(p => p.Id)
                                             .ToListAsync();
        }
    }
}
