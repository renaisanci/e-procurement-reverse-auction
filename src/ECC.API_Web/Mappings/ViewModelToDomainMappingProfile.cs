using AutoMapper;
using ECC.API_Web.Models;
using ECC.EntidadePessoa;


namespace ECC.API_Web.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
      
     
        protected override void Configure()
        {
            Mapper.CreateMap<FornecedorViewModel, Fornecedor>();

        }
    }
}