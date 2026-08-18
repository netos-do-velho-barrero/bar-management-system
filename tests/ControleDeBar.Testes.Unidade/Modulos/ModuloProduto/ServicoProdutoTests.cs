using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ServicoProdutoTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IRepositorioPedidoConta> repositorioPedidoContaMock = null!;
    private ServicoProduto servicoProduto = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        repositorioPedidoContaMock = new Mock<IRepositorioPedidoConta>();

        servicoProduto = new ServicoProduto(
            repositorioProdutoMock.Object,
            repositorioPedidoContaMock.Object
        );
    }

    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void SelecionarPorId_VisualizarDadosDeUmProduto_DeveRetornarDetalhes()
    {
        // CT-PRO-011 [POSITIVO]
        Guid id = Guid.NewGuid();
        Produto produto = new Produto("Cerveja Long Neck", 12.50m);

        repositorioProdutoMock.Setup(r => r.SelecionarPorId(id)).Returns(produto);

        Result<DetalhesProdutoDto> resultado = servicoProduto.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Cerveja Long Neck", resultado.Value.Nome);
        Assert.AreEqual(12.50m, resultado.Value.PrecoVenda);
        repositorioProdutoMock.Verify(r => r.SelecionarPorId(id), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_ListarTodosOsProdutosDoBar_DeveRetornarLista()
    {
        // CT-PRO-012 [POSITIVO]
        List<Produto> produtos =
        [
            new Produto("Cerveja Long Neck", 12.50m),
            new Produto("Caipirinha", 19.90m)
        ];

        repositorioProdutoMock.Setup(r => r.SelecionarTodos()).Returns(produtos);

        List<ListarProdutoDto> resultado = servicoProduto.SelecionarTodos();

        Assert.AreEqual(2, resultado.Count);
        Assert.IsTrue(resultado.Any(p => p.Nome == "Cerveja Long Neck" && p.PrecoVenda == 12.50m));
        Assert.IsTrue(resultado.Any(p => p.Nome == "Caipirinha" && p.PrecoVenda == 19.90m));
        repositorioProdutoMock.Verify(r => r.SelecionarTodos(), Times.Once);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void SelecionarPorId_ProdutoNaoEncontrado_DeveRetornarFalha()
    {
        // CT-PRO-011 [NEGATIVO] — complementa o cenário positivo, garantindo
        // que o serviço não estoura exceção quando o Id não existe.
        Guid id = Guid.NewGuid();

        repositorioProdutoMock.Setup(r => r.SelecionarPorId(id)).Returns((Produto?)null);

        Result<DetalhesProdutoDto> resultado = servicoProduto.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarTodos_SemProdutosCadastrados_DeveRetornarListaVazia()
    {
        // CT-PRO-012 [NEGATIVO] — complementa o cenário positivo com o caso
        // de bar sem nenhum produto cadastrado ainda.
        repositorioProdutoMock.Setup(r => r.SelecionarTodos()).Returns([]);

        List<ListarProdutoDto> resultado = servicoProduto.SelecionarTodos();

        Assert.AreEqual(0, resultado.Count);
    }

    #endregion
}
