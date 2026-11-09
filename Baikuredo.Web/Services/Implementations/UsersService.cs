using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
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
    }
}
