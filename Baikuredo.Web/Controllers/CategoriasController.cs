using Baikuredo.Web.DTOs.Categoria;
using Baikuredo.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Baikuredo.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriasService _categoriasService;

        public CategoriasController(ICategoriasService categoriasService)
        {
            _categoriasService = categoriasService;
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _categoriasService.GetListAsync();

            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Message;

                return View(new List<CategoriaDTO>());
            }

            return View(response.Result);
        }

        // CREATE - Mostrar formulario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Guardar categoría
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoriaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _categoriasService.CreateAsync(dto);

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
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _categoriasService.GetOneAsync(id);

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
        public async Task<IActionResult> Edit(UpdateCategoriaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _categoriasService.UpdateAsync(dto);

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
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _categoriasService.GetOneAsync(id);

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
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _categoriasService.DeleteAsync(id);

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
