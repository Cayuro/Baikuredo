using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Reservations;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    [CustomAuthorize("Reservations")]
    public class ReservationsController : Controller
    {
        private readonly IReservationsService _reservationsService;
        private readonly ISelectListHelper _selectListHelper;

        public ReservationsController(IReservationsService reservationsService, ISelectListHelper selectListHelper)
        {
            _reservationsService = reservationsService;
            _selectListHelper = selectListHelper;
        }

        // INDEX — Listado paginado con filtro
        [HttpGet]
        [CustomAuthorize(permissionCode: "reservations.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _reservationsService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<ReservationDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<ReservationDTO>(new List<ReservationDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "reservations.create")]
        public async Task<IActionResult> Create()
        {
            CreateReservationDTO dto = new CreateReservationDTO
            {
                ExpiresAt = DateTime.Today.AddDays(3),
                Publications = await _selectListHelper.GetAvailablePublicationsSelectListAsync()
            };

            return View(dto);
        }

        // CREATE — Guardar reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "reservations.create")]
        public async Task<IActionResult> Create(CreateReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Publications = await _selectListHelper.GetAvailablePublicationsSelectListAsync();
                return View(dto);
            }

            var response = await _reservationsService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear la reserva.");
                dto.Publications = await _selectListHelper.GetAvailablePublicationsSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // COMPLETE — La moto se vendió
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "reservations.update")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var response = await _reservationsService.CompleteAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EXPIRE — El cliente no volvió, se libera la moto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "reservations.update")]
        public async Task<IActionResult> Expire(Guid id)
        {
            var response = await _reservationsService.ExpireAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Eliminar reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "reservations.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _reservationsService.DeleteAsync(id);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}