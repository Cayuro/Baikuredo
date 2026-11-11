using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.MotorcycleModels;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    [CustomAuthorize("MotorcycleModels")]
    public class MotorcycleModelsController : Controller
    {
        private readonly IMotorcycleModelsService _motorcycleModelsService;
        private readonly ISelectListHelper _selectListHelper;

        public MotorcycleModelsController(IMotorcycleModelsService motorcycleModelsService, ISelectListHelper selectListHelper)
        {
            _motorcycleModelsService = motorcycleModelsService;
            _selectListHelper = selectListHelper;
        }

        // INDEX — Listado paginado con filtro
        [HttpGet]
        [CustomAuthorize(permissionCode: "models.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _motorcycleModelsService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<MotorcycleModelDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<MotorcycleModelDTO>(new List<MotorcycleModelDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "models.create")]
        public async Task<IActionResult> Create()
        {
            CreateMotorcycleModelDTO dto = new CreateMotorcycleModelDTO
            {
                Brands = await _selectListHelper.GetBrandsSelectListAsync()
            };

            return View(dto);
        }

        // CREATE — Guardar modelo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "models.create")]
        public async Task<IActionResult> Create(CreateMotorcycleModelDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Brands = await _selectListHelper.GetBrandsSelectListAsync();
                return View(dto);
            }

            var response = await _motorcycleModelsService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear el modelo.");
                dto.Brands = await _selectListHelper.GetBrandsSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EDIT — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "models.update")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _motorcycleModelsService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            UpdateMotorcycleModelDTO dto = response.Result;
            dto.Brands = await _selectListHelper.GetBrandsSelectListAsync();

            return View(dto);
        }

        // EDIT — Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "models.update")]
        public async Task<IActionResult> Edit(UpdateMotorcycleModelDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Brands = await _selectListHelper.GetBrandsSelectListAsync();
                return View(dto);
            }

            var response = await _motorcycleModelsService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar el modelo.");
                dto.Brands = await _selectListHelper.GetBrandsSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Mostrar confirmación
        [HttpGet]
        [CustomAuthorize(permissionCode: "models.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _motorcycleModelsService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // DELETE — Confirmar eliminación
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "models.delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _motorcycleModelsService.DeleteAsync(id);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // TOGGLE — Activar/Desactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "models.update")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var response = await _motorcycleModelsService.ToggleAsync(id);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = response.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}