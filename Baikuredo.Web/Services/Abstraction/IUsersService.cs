using Baikuredo.Web.Core;
using Baikuredo.Web.DTOs.Users;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IUsersService
    {
        Task<Response<List<UserDTO>>> GetListAsync();
        Task<Response<UserDTO>> CreateAsync(CreateUserDTO dto);
    }
}
