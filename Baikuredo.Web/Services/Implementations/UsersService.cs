using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.Data;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Users;
using Baikuredo.Web.Services.Abstractions;

namespace Baikuredo.Web.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UsersService(DataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Response<List<UserDTO>>> GetListAsync()
        {
            try
            {
                List<UserDTO> list = await _context.Users
                    .AsNoTracking()
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        Email = u.Email ?? string.Empty,
                        Document = u.Document,
                        Phone = u.PhoneNumber,
                        RoleName = u.BaikuredoRole != null ? u.BaikuredoRole.Name : string.Empty
                    })
                    .ToListAsync();

                return Response<List<UserDTO>>.Success(list);
            }
            catch (Exception ex)
            {
                return Response<List<UserDTO>>.Failure(ex);
            }
        }

        public async Task<Response<PaginationResponse<UserDTO>>> GetPaginatedListAsync(PaginationRequest request)
        {
            try
            {
                IQueryable<User> query = _context.Users.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filter = request.Filter.ToLower();
                    query = query.Where(u => u.FirstName.ToLower().Contains(filter)
                                          || u.LastName.ToLower().Contains(filter)
                                          || u.Document.ToLower().Contains(filter)
                                          || u.Email!.ToLower().Contains(filter)
                                          || u.BaikuredoRole!.Name.ToLower().Contains(filter));
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)request.RecordsPerPage);

                var items = await query
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((request.Page - 1) * request.RecordsPerPage)
                    .Take(request.RecordsPerPage)
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FullName = u.FirstName + " " + u.LastName,
                        Email = u.Email ?? string.Empty,
                        Document = u.Document,
                        Phone = u.PhoneNumber,
                        RoleName = u.BaikuredoRole != null ? u.BaikuredoRole.Name : string.Empty
                    })
                    .ToListAsync();

                var pagedList = new PagedList<UserDTO>(items, totalCount, request.Page, request.RecordsPerPage);

                var paginationResponse = new PaginationResponse<UserDTO>
                {
                    CurrentPage = request.Page,
                    RecordsPerPage = request.RecordsPerPage,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Filter = request.Filter,
                    List = pagedList
                };

                return Response<PaginationResponse<UserDTO>>.Success(paginationResponse);
            }
            catch (Exception ex)
            {
                return Response<PaginationResponse<UserDTO>>.Failure(ex);
            }
        }

        public async Task<Response<UserDTO>> CreateAsync(CreateUserDTO dto)
        {
            try
            {
                string email = dto.Email.Trim();
                string document = dto.Document.Trim();

                User? existingByEmail = await _userManager.FindByEmailAsync(email);
                if (existingByEmail is not null)
                {
                    return Response<UserDTO>.Failure("Ya existe un usuario con ese correo.");
                }

                bool documentExists = await _context.Users.AnyAsync(u => u.Document.ToLower() == document.ToLower());
                if (documentExists)
                {
                    return Response<UserDTO>.Failure("Ya existe un usuario con ese documento.");
                }

                bool roleExists = await _context.BaikuredoRoles.AnyAsync(r => r.Id == dto.BaikuredoRoleId);
                if (!roleExists)
                {
                    return Response<UserDTO>.Failure("El rol seleccionado no existe.");
                }

                User user = _mapper.Map<User>(dto);
                user.Email = email;
                user.UserName = email;
                user.Document = document;
                user.FirstName = dto.FirstName.Trim();
                user.LastName = dto.LastName.Trim();
                user.PhoneNumber = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
                user.EmailConfirmed = true;

                IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    string errors = string.Join(" ", result.Errors.Select(e => e.Description));
                    return Response<UserDTO>.Failure(errors);
                }

                string? roleName = await _context.BaikuredoRoles
                    .Where(r => r.Id == user.BaikuredoRoleId)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                UserDTO resultDto = new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? email,
                    Document = user.Document,
                    Phone = user.PhoneNumber,
                    RoleName = roleName ?? string.Empty
                };

                return Response<UserDTO>.Success(resultDto, "Usuario creado exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<UserDTO>.Failure(ex);
            }
        }

        public async Task<Response<UpdateUserDTO>> GetOneAsync(string id)
        {
            try
            {
                User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

                if (user is null)
                {
                    return Response<UpdateUserDTO>.Failure("No se encontró el usuario.");
                }

                UpdateUserDTO dto = _mapper.Map<UpdateUserDTO>(user);

                return Response<UpdateUserDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<UpdateUserDTO>.Failure(ex);
            }
        }

        public async Task<Response<UserDTO>> UpdateAsync(UpdateUserDTO dto)
        {
            try
            {
                User? user = await _userManager.FindByIdAsync(dto.Id);
                if (user is null)
                {
                    return Response<UserDTO>.Failure("El usuario no existe.");
                }

                string email = dto.Email.Trim();
                string document = dto.Document.Trim();

                User? existingByEmail = await _userManager.FindByEmailAsync(email);
                if (existingByEmail is not null && existingByEmail.Id != user.Id)
                {
                    return Response<UserDTO>.Failure("Ya existe un usuario con ese correo.");
                }

                bool documentExists = await _context.Users
                    .AnyAsync(u => u.Document.ToLower() == document.ToLower() && u.Id != user.Id);
                if (documentExists)
                {
                    return Response<UserDTO>.Failure("Ya existe un usuario con ese documento.");
                }

                bool roleExists = await _context.BaikuredoRoles.AnyAsync(r => r.Id == dto.BaikuredoRoleId);
                if (!roleExists)
                {
                    return Response<UserDTO>.Failure("El rol seleccionado no existe.");
                }

                user.Document = document;
                user.FirstName = dto.FirstName.Trim();
                user.LastName = dto.LastName.Trim();
                user.PhoneNumber = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
                user.BaikuredoRoleId = dto.BaikuredoRoleId!.Value;
                user.Email = email;
                user.UserName = email;
                user.NormalizedEmail = _userManager.NormalizeEmail(email);
                user.NormalizedUserName = _userManager.NormalizeName(email);

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    string errors = string.Join(" ", result.Errors.Select(e => e.Description));
                    return Response<UserDTO>.Failure(errors);
                }

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    IdentityResult passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                    if (!passwordResult.Succeeded)
                    {
                        string errors = string.Join(" ", passwordResult.Errors.Select(e => e.Description));
                        return Response<UserDTO>.Failure(errors);
                    }
                }

                string? roleName = await _context.BaikuredoRoles
                    .Where(r => r.Id == user.BaikuredoRoleId)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                UserDTO resultDto = new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? email,
                    Document = user.Document,
                    Phone = user.PhoneNumber,
                    RoleName = roleName ?? string.Empty
                };

                return Response<UserDTO>.Success(resultDto, "Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return Response<UserDTO>.Failure(ex);
            }
        }
    }
}
