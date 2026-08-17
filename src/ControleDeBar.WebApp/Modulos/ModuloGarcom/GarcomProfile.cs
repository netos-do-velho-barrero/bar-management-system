using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

namespace ControleDeBar.WebApp.Modulos.ModuloGarcom;

public sealed class GarcomProfile : Profile
{
    public GarcomProfile()
    {
        CreateMap<CadastrarGarcomViewModel, CadastrarGarcomDto>();

        CreateMap<EditarGarcomViewModel, EditarGarcomDto>();

        CreateMap<ListarGarcomDto, ListarGarcomViewModel>();

        CreateMap<DetalhesGarcomDto, DetalhesGarcomViewModel>();

        CreateMap<DetalhesGarcomDto, EditarGarcomViewModel>();
    }
}
