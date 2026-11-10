using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Roles;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        // INDEX — Listado paginado con filtro
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _rolesService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<RoleListDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<RoleListDTO>(new List<RoleListDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario con todos los permisos sin marcar
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var response = await _rolesService.GetNewFormAsync();

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // CREATE — Guardar rol y sus permisos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Permissions = await _rolesService.RebuildPermissionsAsync(dto.Permissions);
                return View(dto);
            }

            var response = await _rolesService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear el rol.");
                dto.Permissions = await _rolesService.RebuildPermissionsAsync(dto.Permissions);
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EDIT — Mostrar formulario con los permisos del rol marcados
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _rolesService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // EDIT — Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleFormDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Permissions = await _rolesService.RebuildPermissionsAsync(dto.Permissions);
                return View(dto);
            }

            var response = await _rolesService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar el rol.");
                dto.Permissions = await _rolesService.RebuildPermissionsAsync(dto.Permissions);
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Eliminar rol (la confirmación se hace en el listado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _rolesService.DeleteAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
