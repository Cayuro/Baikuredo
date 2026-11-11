using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Users;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly ISelectListHelper _selectListHelper;

        public UsersController(IUsersService usersService, ISelectListHelper selectListHelper)
        {
            _usersService = usersService;
            _selectListHelper = selectListHelper;
        }

        [HttpGet]
        [CustomAuthorize(permissionCode: "users.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _usersService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<UserDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<UserDTO>(new List<UserDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permissionCode: "users.create")]
        public async Task<IActionResult> Create()
        {
            CreateUserDTO dto = new CreateUserDTO
            {
                Roles = await _selectListHelper.GetRolesSelectListAsync()
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "users.create")]
        public async Task<IActionResult> Create(CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Roles = await _selectListHelper.GetRolesSelectListAsync();
                return View(dto);
            }

            var response = await _usersService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear el usuario.");
                dto.Roles = await _selectListHelper.GetRolesSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [CustomAuthorize(permissionCode: "users.update")]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await _usersService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            UpdateUserDTO dto = response.Result;
            dto.Roles = await _selectListHelper.GetRolesSelectListAsync();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "users.update")]
        public async Task<IActionResult> Edit(UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Roles = await _selectListHelper.GetRolesSelectListAsync();
                return View(dto);
            }

            var response = await _usersService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar el usuario.");
                dto.Roles = await _selectListHelper.GetRolesSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
