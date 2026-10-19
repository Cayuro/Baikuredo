using AutoMapper;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Categories;
using Baikuredo.Web.DTOs.Brands;

namespace Baikuredo.Web.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapeos para Categoria

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>().ReverseMap();

            // Mapeos para Marca (Descomentar cuando Fraynner suba la entidad Marca)
            // CreateMap<Marca, MarcaDTO>().ReverseMap();
            // CreateMap<CreateMarcaDTO, Marca>();
            // CreateMap<UpdateMarcaDTO, Marca>().ReverseMap();
        }
    }
}