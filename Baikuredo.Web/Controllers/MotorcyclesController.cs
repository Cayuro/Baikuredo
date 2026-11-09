using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Motorcycles;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class MotorcyclesController : Controller
    {
        private readonly IMotorcyclesService _motorcyclesService;
        private readonly ISelectListHelper _selectListHelper;

        public MotorcyclesController(IMotorcyclesService motorcyclesService, ISelectListHelper selectListHelper)
        {
            _motorcyclesService = motorcyclesService;
            _selectListHelper = selectListHelper;
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

            var response = await _motorcyclesService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<MotorcycleDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<MotorcycleDTO>(new List<MotorcycleDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
            ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
            ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();

            CreateMotorcycleDTO dto = new CreateMotorcycleDTO();

            return View(dto);
        }

        // CREATE — Guardar moto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMotorcycleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
                ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
                ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();
                return View(dto);
            }

            var response = await _motorcyclesService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible registrar la moto.");
                ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
                ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
                ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EDIT — Mostrar formulario
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _motorcyclesService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
            ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
            ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();

            UpdateMotorcycleDTO dto = response.Result;

            return View(dto);
        }

        // EDIT — Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMotorcycleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
                ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
                ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();
                return View(dto);
            }

            var response = await _motorcyclesService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar la moto.");
                ViewBag.MotorcycleModels = await _selectListHelper.GetMotorcycleModelsSelectListAsync();
                ViewBag.Categories = await _selectListHelper.GetCategoriesSelectListAsync();
                ViewBag.Owners = await _selectListHelper.GetOwnersSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Mostrar confirmación
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _motorcyclesService.GetDetailAsync(id);

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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _motorcyclesService.DeleteAsync(id);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}