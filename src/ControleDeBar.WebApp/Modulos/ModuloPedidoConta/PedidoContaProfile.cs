using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;

namespace ControleDeBar.WebApp.Modulos.ModuloPedidoConta;

public sealed class PedidoContaProfile : Profile
{
    public PedidoContaProfile()
    {
        CreateMap<AdicionarPedidoContaViewModel, AdicionarPedidoContaDto>();

        CreateMap<EditarQuantidadePedidoContaViewModel, EditarQuantidadePedidoContaDto>();

        CreateMap<ListarPedidoContaDto, ListarPedidoContaViewModel>();

        CreateMap<OpcaoProdutoPedidoDto, OpcaoProdutoPedidoViewModel>();
    }
}
