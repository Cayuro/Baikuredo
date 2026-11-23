using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Catalog;
using Baikuredo.Web.DTOs.Interests;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    // Catálogo público de motocicletas disponibles (F001, F002, F003).
    // No exige iniciar sesión ([AllowAnonymous]).
    [AllowAnonymous]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly IInterestsService _interestsService;

        public CatalogController(ICatalogService catalogService, IInterestsService interestsService)
        {
            _catalogService = catalogService;
            _interestsService = interestsService;
        }

        // INDEX — Listado paginado de publicaciones en estado "Disponible" con su foto de portada (F001)
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 12,
                Filter = filter
            };

            var response = await _catalogService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<CatalogItemDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 12,
                    TotalCount = 0,
                    List = new PagedList<CatalogItemDTO>(new List<CatalogItemDTO>(), 0, 1, 12)
                });
            }

            return View(response.Result);
        }

        // DETAILS — Detalle público de una publicación con todas sus fotos, sin datos internos (F002)
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _catalogService.GetDetailAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // REGISTER INTEREST — Recibe el formulario público de interés con validaciones y token antifalsificación (F003)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterInterest(CreateInterestDTO interest)
        {
            if (!ModelState.IsValid)
            {
                var detailResponse = await _catalogService.GetDetailAsync(interest.MotorcyclePublicationId);

                if (!detailResponse.IsSuccess || detailResponse.Result == null)
                {
                    TempData["Error"] = detailResponse.Message ?? "La publicación ya no está disponible.";
                    return RedirectToAction(nameof(Index));
                }

                detailResponse.Result.Interest = interest;
                return View(nameof(Details), detailResponse.Result);
            }

            var response = await _interestsService.CreateAsync(interest);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible registrar tu solicitud.");

                var detailResponse = await _catalogService.GetDetailAsync(interest.MotorcyclePublicationId);

                if (!detailResponse.IsSuccess || detailResponse.Result == null)
                {
                    TempData["Error"] = response.Message;
                    return RedirectToAction(nameof(Index));
                }

                detailResponse.Result.Interest = interest;
                return View(nameof(Details), detailResponse.Result);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Details), new { id = interest.MotorcyclePublicationId });
        }
    }
}
