using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Categories;
using Baikuredo.Web.Services.Abstractions;
using Baikuredo.Web.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    [CustomAuthorize("Categories")]
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        // READ
        [HttpGet]
        [CustomAuthorize(permissionCode: "categories.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _categoriesService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<CategoryDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<CategoryDTO>(new List<CategoryDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE - Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "categories.create")]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Guardar categoría
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "categories.create")]
        public async Task<IActionResult> Create(CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _categoriesService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    response.Message ?? "No fue posible crear la categoría."
                );

                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // UPDATE - Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "categories.update")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _categoriesService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;

                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // UPDATE - Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "categories.update")]
        public async Task<IActionResult> Edit(UpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _categoriesService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    response.Message ?? "No fue posible actualizar la categoría."
                );

                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE - Mostrar confirmación
        [HttpGet]
        [CustomAuthorize(permissionCode: "categories.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _categoriesService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;

                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // DELETE - Confirmar eliminación
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "categories.delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _categoriesService.DeleteAsync(id);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;

                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "categories.update")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var response = await _categoriesService.ToggleAsync(id);

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