using Baikuredo.Web.Core.Attributes;
using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Publications;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    [CustomAuthorize("Publications")]
    public class PublicationsController : Controller
    {
        private readonly IPublicationsService _publicationsService;
        private readonly ISelectListHelper _selectListHelper;

        public PublicationsController(IPublicationsService publicationsService, ISelectListHelper selectListHelper)
        {
            _publicationsService = publicationsService;
            _selectListHelper = selectListHelper;
        }

        // INDEX — Listado paginado con filtro
        [HttpGet]
        [CustomAuthorize(permissionCode: "publications.show")]
        public async Task<IActionResult> Index([FromQuery] int? page, [FromQuery] int? recordsPerPage, [FromQuery] string? filter)
        {
            PaginationRequest request = new()
            {
                Page = page ?? 1,
                RecordsPerPage = recordsPerPage ?? 15,
                Filter = filter
            };

            var response = await _publicationsService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<PublicationDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<PublicationDTO>(new List<PublicationDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // DETAILS — Ver publicación con fotos, historial y reservas
        [HttpGet]
        [CustomAuthorize(permissionCode: "publications.show")]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _publicationsService.GetDetailAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "publications.create")]
        public async Task<IActionResult> Create()
        {
            CreatePublicationDTO dto = new CreatePublicationDTO
            {
                Motorcycles = await _selectListHelper.GetMotorcyclesToPublishSelectListAsync()
            };

            return View(dto);
        }

        // CREATE — Guardar publicación
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "publications.create")]
        public async Task<IActionResult> Create(CreatePublicationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Motorcycles = await _selectListHelper.GetMotorcyclesToPublishSelectListAsync();
                return View(dto);
            }

            var response = await _publicationsService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear la publicación.");
                dto.Motorcycles = await _selectListHelper.GetMotorcyclesToPublishSelectListAsync();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            // Se va al detalle para que pueda subir las fotos
            return RedirectToAction(nameof(Details), new { id = response.Result });
        }

        // EDIT — Mostrar formulario
        [HttpGet]
        [CustomAuthorize(permissionCode: "publications.update")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _publicationsService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            UpdatePublicationDTO dto = response.Result;
            dto.Statuses = _selectListHelper.GetPublicationStatusesSelectList();

            return View(dto);
        }

        // EDIT — Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "publications.update")]
        public async Task<IActionResult> Edit(UpdatePublicationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Statuses = _selectListHelper.GetPublicationStatusesSelectList();
                return View(dto);
            }

            var response = await _publicationsService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar la publicación.");
                dto.Statuses = _selectListHelper.GetPublicationStatusesSelectList();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Details), new { id = dto.Id });
        }

        // DELETE — Mostrar confirmación
        [HttpGet]
        [CustomAuthorize(permissionCode: "publications.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _publicationsService.GetDetailAsync(id);

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
        [CustomAuthorize(permissionCode: "publications.delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _publicationsService.DeleteAsync(id);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // FOTOS — Subir una foto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "publications.update")]
        public async Task<IActionResult> AddPhoto(Guid publicationId, IFormFile photo)
        {
            var response = await _publicationsService.AddPhotoAsync(publicationId, photo);

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = response.Message;
            }

            return RedirectToAction(nameof(Details), new { id = publicationId });
        }

        // FOTOS — Eliminar una foto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "publications.update")]
        public async Task<IActionResult> DeletePhoto(Guid photoId, Guid publicationId)
        {
            var response = await _publicationsService.DeletePhotoAsync(photoId);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Details), new { id = publicationId });
        }

        // FOTOS — Elegir la portada
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(permissionCode: "publications.update")]
        public async Task<IActionResult> SetMainPhoto(Guid photoId, Guid publicationId)
        {
            var response = await _publicationsService.SetMainPhotoAsync(photoId);
            TempData[response.IsSuccess ? "Success" : "Error"] = response.Message;

            return RedirectToAction(nameof(Details), new { id = publicationId });
        }
    }
}