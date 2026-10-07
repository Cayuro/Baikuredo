using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Marca;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class MarcasController : Controller
    {
        private readonly IMarcasService _marcasService;

        public MarcasController(IMarcasService marcasService)
        {
            _marcasService = marcasService;
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

            var response = await _marcasService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<MarcaDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<MarcaDTO>(new List<MarcaDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE — Guardar marca
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMarcaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _marcasService.CreateAsync(dto);

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
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _marcasService.GetOneAsync(id);

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
        public async Task<IActionResult> Edit(UpdateMarcaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _marcasService.UpdateAsync(dto);

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
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _marcasService.GetOneAsync(id);

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
            var response = await _marcasService.DeleteAsync(id);

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
        public async Task<IActionResult> Toggle(Guid id)
        {
            var response = await _marcasService.ToggleAsync(id);

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
