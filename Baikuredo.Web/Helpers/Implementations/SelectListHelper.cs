using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core.Enums;
using Baikuredo.Web.Data;
using Baikuredo.Web.Helpers.Abstractions;

namespace Baikuredo.Web.Helpers.Implementations
{
    public class SelectListHelper : ISelectListHelper
    {
        private readonly DataContext _context;

        public SelectListHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetBrandsSelectListAsync()
        {
            return await _context.Brands.Where(b => b.IsActive)
                                        .OrderBy(b => b.Name)
                                        .Select(b => new SelectListItem
                                        {
                                            Text = b.Name,
                                            Value = b.Id.ToString()
                                        }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCategoriesSelectListAsync()
        {
            return await _context.Categories.Where(c => c.IsActive)
                                            .OrderBy(c => c.Name)
                                            .Select(c => new SelectListItem
                                            {
                                                Text = c.Name,
                                                Value = c.Id.ToString()
                                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetRolesSelectListAsync()
        {
            return await _context.BaikuredoRoles.OrderBy(r => r.Name)
                                                .Select(r => new SelectListItem
                                                {
                                                    Text = r.Name,
                                                    Value = r.Id.ToString()
                                                }).ToListAsync();
        }

        // Modelos activos de marcas activas. Se muestra "Marca Modelo", ej: "Yamaha MT-09"
        public async Task<List<SelectListItem>> GetMotorcycleModelsSelectListAsync()
        {
            return await _context.MotorcycleModels.Where(m => m.IsActive && m.Brand!.IsActive)
                                                  .OrderBy(m => m.Brand!.Name)
                                                  .ThenBy(m => m.Name)
                                                  .Select(m => new SelectListItem
                                                  {
                                                      Text = m.Brand!.Name + " " + m.Name,
                                                      Value = m.Id.ToString()
                                                  }).ToListAsync();
        }

        // Propietarios. Se muestra "Nombre Apellido - Documento"
        public async Task<List<SelectListItem>> GetOwnersSelectListAsync()
        {
            return await _context.Owners.OrderBy(o => o.FirstName)
                                        .ThenBy(o => o.LastName)
                                        .Select(o => new SelectListItem
                                        {
                                            Text = o.FirstName + " " + o.LastName + " - " + o.DocumentType + " " + o.Document,
                                            Value = o.Id.ToString()
                                        }).ToListAsync();
        }

        // Motos que se pueden publicar: las que no tienen una publicación Disponible o Reservada
        public async Task<List<SelectListItem>> GetMotorcyclesToPublishSelectListAsync()
        {
            return await _context.Motorcycles.Where(m => !m.Publications!.Any(p => p.Status == PublicationStatus.Available || p.Status == PublicationStatus.Reserved))
                                             .OrderBy(m => m.Plate)
                                             .Select(m => new SelectListItem
                                             {
                                                 Text = m.Plate + " - " + m.MotorcycleModel!.Brand!.Name + " " + m.MotorcycleModel!.Name,
                                                 Value = m.Id.ToString()
                                             }).ToListAsync();
        }

        // Publicaciones disponibles para reservar
        public async Task<List<SelectListItem>> GetAvailablePublicationsSelectListAsync()
        {
            return await _context.MotorcyclePublications.Where(p => p.Status == PublicationStatus.Available)
                                                        .OrderBy(p => p.Motorcycle!.Plate)
                                                        .Select(p => new SelectListItem
                                                        {
                                                            Text = p.Motorcycle!.Plate + " - " + p.Motorcycle!.MotorcycleModel!.Brand!.Name + " " + p.Motorcycle!.MotorcycleModel!.Name,
                                                            Value = p.Id.ToString()
                                                        }).ToListAsync();
        }

        // Tipos de documento (lista fija)
        public List<SelectListItem> GetDocumentTypesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Cédula de ciudadanía", Value = "CC" },
                new SelectListItem { Text = "Cédula de extranjería", Value = "CE" },
                new SelectListItem { Text = "NIT", Value = "NIT" },
                new SelectListItem { Text = "Pasaporte", Value = "PAS" },
            };
        }

        // Estados de una publicación (lista fija, sale del enum)
        public List<SelectListItem> GetPublicationStatusesSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Disponible", Value = PublicationStatus.Available.ToString() },
                new SelectListItem { Text = "Reservada", Value = PublicationStatus.Reserved.ToString() },
                new SelectListItem { Text = "Vendida", Value = PublicationStatus.Sold.ToString() },
                new SelectListItem { Text = "Cancelada", Value = PublicationStatus.Cancelled.ToString() },
            };
        }
    }
}
