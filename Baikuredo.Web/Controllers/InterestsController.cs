using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Interests;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    // Parte administrativa de los interesados (F011).
    // El registro del interesado (F003) lo hace el catálogo público llamando a IInterestsService.CreateAsync.
    public class InterestsController : Controller
    {
        private readonly IInterestsService _interestsService;

        public InterestsController(IInterestsService interestsService)
        {
            _interestsService = interestsService;
        }

        // INDEX — Listado paginado con filtro (nombre, correo, teléfono o placa)
        [HttpGet]
        [CustomAuthorize(permissionCode: "interests.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _interestsService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<InterestDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<InterestDTO>(new List<InterestDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // TOGGLE — Marcar como atendido o volver a pendiente
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "interests.update")]
        public async Task<IActionResult> ToggleAttended(Guid id)
        {
            var response = await _interestsService.ToggleAttendedAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Eliminar interesado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "interests.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _interestsService.DeleteAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
