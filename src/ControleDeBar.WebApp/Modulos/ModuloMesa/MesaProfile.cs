using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;

namespace ControleDeBar.WebApp.Modulos.ModuloMesa;

public sealed class MesaProfile : Profile
{
    public MesaProfile()
    {
        CreateMap<CadastrarMesaViewModel, CadastrarMesaDto>();

        CreateMap<EditarMesaViewModel, EditarMesaDto>();

        CreateMap<ListarMesaDto, ListarMesaViewModel>();

        CreateMap<DetalhesMesaDto, DetalhesMesaViewModel>();

        CreateMap<DetalhesMesaDto, EditarMesaViewModel>();
    }
}
