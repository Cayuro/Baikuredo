using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Brands;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    [CustomAuthorize("brands.index")]
    public class BrandsController : Controller
    {
        private readonly IBrandsService _brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        // INDEX — Listado paginado con filtro
        [HttpGet]
        [CustomAuthorize(permissionCode: "brands.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _brandsService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<BrandDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<BrandDTO>(new List<BrandDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "brands.create")]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE — Guardar marca
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "brands.create")]
        public async Task<IActionResult> Create(CreateBrandDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _brandsService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    response.Message ?? "No fue posible crear la marca."
                );

                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EDIT — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "brands.update")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _brandsService.GetOneAsync(id);

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
        [CustomAuthorize(permissionCode: "brands.update")]
        public async Task<IActionResult> Edit(UpdateBrandDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _brandsService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    response.Message ?? "No fue posible actualizar la marca."
                );

                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Mostrar confirmación
        [HttpGet]
        [CustomAuthorize(permissionCode: "brands.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _brandsService.GetOneAsync(id);

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
        [CustomAuthorize(permissionCode: "brands.delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _brandsService.DeleteAsync(id);

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
        [CustomAuthorize(permissionCode: "brands.update")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var response = await _brandsService.ToggleAsync(id);

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