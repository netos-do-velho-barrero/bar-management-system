using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public sealed class ContaProfile : Profile
{
    public ContaProfile()
    {
        CreateMap<AbrirContaViewModel, AbrirContaDto>();

        CreateMap<EditarContaViewModel, EditarContaDto>();

        CreateMap<ListarContaDto, ListarContaViewModel>();

        CreateMap<DetalhesContaDto, DetalhesContaViewModel>();

        CreateMap<ItemPedidoDaContaDto, ItemPedidoDaContaViewModel>();

        CreateMap<OpcaoMesaContaDto, OpcaoMesaContaViewModel>();

        CreateMap<OpcaoGarcomContaDto, OpcaoGarcomContaViewModel>();

        CreateMap<DetalhesContaDto, EditarContaViewModel>();
    }
}
