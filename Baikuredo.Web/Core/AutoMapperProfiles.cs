using AutoMapper;
using Baikuredo.Web.Data.Entities;
using Baikuredo.Web.DTOs.Categoria;

namespace Baikuredo.Web.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            CreateMap<CreateCategoriaDTO, Categoria>();
            CreateMap<UpdateCategoriaDTO, Categoria>().ReverseMap();
        }
    }
}
