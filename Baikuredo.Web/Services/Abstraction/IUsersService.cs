using Baikuredo.Web.Core;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Users;

namespace Baikuredo.Web.Services.Abstractions
{
    public interface IUsersService
    {
        Task<Response<List<UserDTO>>> GetListAsync();
        Task<Response<PaginationResponse<UserDTO>>> GetPaginatedListAsync(PaginationRequest request);
        Task<Response<UserDTO>> CreateAsync(CreateUserDTO dto);
    }
}
