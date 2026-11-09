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
        public async Task<IActionResult> Index()
        {
            var response = await _usersService.GetListAsync();

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new List<UserDTO>());
            }

            return View(response.Result);
        }

        [HttpGet]
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
    }
}
