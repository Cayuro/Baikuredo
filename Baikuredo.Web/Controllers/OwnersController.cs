using Baikuredo.Web.Core.Pagination;
using Baikuredo.Web.DTOs.Owners;
using Baikuredo.Web.Helpers.Abstractions;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class OwnersController : Controller
    {
        private readonly IOwnersService _ownersService;
        private readonly ISelectListHelper _selectListHelper;

        public OwnersController(IOwnersService ownersService, ISelectListHelper selectListHelper)
        {
            _ownersService = ownersService;
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

            var response = await _ownersService.GetPaginatedListAsync(request);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return View(new PaginationResponse<OwnerDTO>
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    RecordsPerPage = 15,
                    TotalCount = 0,
                    List = new PagedList<OwnerDTO>(new List<OwnerDTO>(), 0, 1, 15)
                });
            }

            return View(response.Result);
        }

        // CREATE — Mostrar formulario
        [HttpGet]
        public IActionResult Create()
        {
            CreateOwnerDTO dto = new CreateOwnerDTO
            {
                DocumentTypes = _selectListHelper.GetDocumentTypesSelectList()
            };

            return View(dto);
        }

        // CREATE — Guardar propietario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOwnerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.DocumentTypes = _selectListHelper.GetDocumentTypesSelectList();
                return View(dto);
            }

            var response = await _ownersService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible crear el propietario.");
                dto.DocumentTypes = _selectListHelper.GetDocumentTypesSelectList();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // EDIT — Mostrar formulario
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _ownersService.GetOneAsync(id);

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            UpdateOwnerDTO dto = response.Result;
            dto.DocumentTypes = _selectListHelper.GetDocumentTypesSelectList();

            return View(dto);
        }

        // EDIT — Guardar cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateOwnerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.DocumentTypes = _selectListHelper.GetDocumentTypesSelectList();
                return View(dto);
            }

            var response = await _ownersService.UpdateAsync(dto);

            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "No fue posible actualizar el propietario.");
                dto.DocumentTypes = _selectListHelper.GetDocumentTypesSelectList();
                return View(dto);
            }

            TempData["Success"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        // DELETE — Mostrar confirmación
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _ownersService.GetOneAsync(id);

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
            var response = await _ownersService.DeleteAsync(id);

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
