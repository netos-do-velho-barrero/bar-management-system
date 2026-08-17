using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;

namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public sealed class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<CadastrarProdutoViewModel, CadastrarProdutoDto>();

        CreateMap<EditarProdutoViewModel, EditarProdutoDto>();

        CreateMap<ListarProdutoDto, ListarProdutoViewModel>();

        CreateMap<DetalhesProdutoDto, DetalhesProdutoViewModel>();

        CreateMap<DetalhesProdutoDto, EditarProdutoViewModel>();
    }
}
