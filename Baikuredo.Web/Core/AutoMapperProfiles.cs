using AutoMapper;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Categoria;
using Baikuredo.Web.DTOs.Marca;

namespace Baikuredo.Web.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapeos para Categoria

            CreateMap<Category, CategoriaDTO>().ReverseMap();
            CreateMap<CreateCategoriaDTO, Category>();
            CreateMap<UpdateCategoriaDTO, Category>().ReverseMap();

            // Mapeos para Marca (Descomentar cuando Fraynner suba la entidad Marca)
            // CreateMap<Marca, MarcaDTO>().ReverseMap();
            // CreateMap<CreateMarcaDTO, Marca>();
            // CreateMap<UpdateMarcaDTO, Marca>().ReverseMap();
        }
    }
}