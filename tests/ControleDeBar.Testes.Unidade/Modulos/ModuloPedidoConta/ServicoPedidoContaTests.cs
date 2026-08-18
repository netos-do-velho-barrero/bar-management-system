using ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloPedidoConta;

[TestClass]
public sealed class ServicoPedidoContaTests
{
    private Mock<IRepositorioPedidoConta> repositorioPedidoContaMock = null!;
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private ServicoPedidoConta servicoPedidoConta = null!;
    private Guid userId;

    [TestInitialize]
    public void Setup()
    {
        repositorioPedidoContaMock = new Mock<IRepositorioPedidoConta>();
        repositorioContaMock = new Mock<IRepositorioConta>();
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        userId = Guid.NewGuid();
        provedorDeUsuarioMock.Setup(p => p.Id).Returns(userId);

        servicoPedidoConta = new ServicoPedidoConta(
            repositorioPedidoContaMock.Object,
            repositorioContaMock.Object,
            repositorioProdutoMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    #region --- CT-PED-005: Adicionar pedido sem selecionar o produto ---

    [TestMethod]
    public void Adicionar_SemSelecionarProduto_DeveRetornarFalha()
    {
        // CT-PED-005 [NEGATIVO]
        // Arranjo
        Guid contaId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        Conta conta = new Conta { UserId = userId, Situacao = SituacaoConta.Aberta };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(contaId)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(contaId)).Returns(conta);
        repositorioProdutoMock.Setup(r => r.SelecionarPorIdSemFiltro(produtoId)).Returns((Produto?)null);
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(produtoId)).Returns((Produto?)null);

        AdicionarPedidoContaDto dto = new AdicionarPedidoContaDto(
            ContaId: contaId,
            ProdutoId: produtoId,
            Quantidade: 2
        );

        // Ação
        Result resultado = servicoPedidoConta.Adicionar(dto);

        // Asserção
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "Selecione um produto válido."));
        repositorioPedidoContaMock.Verify(r => r.Cadastrar(It.IsAny<PedidoConta>()), Times.Never);
    }

    #endregion

    #region --- CT-PED-002 / 003 / 004: Adicionar produto com quantidade inválida ---

    [TestMethod]
    public void Adicionar_ComQuantidadeIgualAZero_DeveRetornarFalha()
    {
        // CT-PED-003 [NEGATIVO] — validação de domínio propagada pelo serviço
        Guid contaId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        Conta conta = new Conta { UserId = userId, Situacao = SituacaoConta.Aberta };
        Produto produto = new Produto("Cerveja Long Neck", 12.50m) { UserId = userId };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(contaId)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(contaId)).Returns(conta);
        repositorioProdutoMock.Setup(r => r.SelecionarPorIdSemFiltro(produtoId)).Returns(produto);
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(produtoId)).Returns(produto);

        AdicionarPedidoContaDto dto = new AdicionarPedidoContaDto(
            ContaId: contaId,
            ProdutoId: produtoId,
            Quantidade: 0
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "O campo \"Quantidade\" deve ser um número positivo."));
        repositorioPedidoContaMock.Verify(r => r.Cadastrar(It.IsAny<PedidoConta>()), Times.Never);
    }

    [TestMethod]
    public void Adicionar_ComQuantidadeNegativa_DeveRetornarFalha()
    {
        // CT-PED-004 [NEGATIVO]
        Guid contaId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        Conta conta = new Conta { UserId = userId, Situacao = SituacaoConta.Aberta };
        Produto produto = new Produto("Cerveja Long Neck", 12.50m) { UserId = userId };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(contaId)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(contaId)).Returns(conta);
        repositorioProdutoMock.Setup(r => r.SelecionarPorIdSemFiltro(produtoId)).Returns(produto);
        repositorioProdutoMock.Setup(r => r.SelecionarPorId(produtoId)).Returns(produto);

        AdicionarPedidoContaDto dto = new AdicionarPedidoContaDto(
            ContaId: contaId,
            ProdutoId: produtoId,
            Quantidade: -1
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "O campo \"Quantidade\" deve ser um número positivo."));
        repositorioPedidoContaMock.Verify(r => r.Cadastrar(It.IsAny<PedidoConta>()), Times.Never);
    }

    #endregion

    #region --- CT-PED-012: Alterar a quantidade de um pedido para zero ---

    [TestMethod]
    public void EditarQuantidade_AlterarQuantidadeParaZero_DeveRetornarFalha()
    {
        // CT-PED-012 [NEGATIVO]
        // Arranjo
        Guid pedidoId = Guid.NewGuid();

        Conta conta = new Conta { UserId = userId, Situacao = SituacaoConta.Aberta };
        Produto produto = new Produto("Cerveja Long Neck", 12.50m) { UserId = userId };
        PedidoConta pedidoExistente = new PedidoConta(produto, 5)
        {
            UserId = userId,
            Conta = conta
        };

        repositorioPedidoContaMock.Setup(r => r.SelecionarPorIdSemFiltro(pedidoId)).Returns(pedidoExistente);
        repositorioPedidoContaMock.Setup(r => r.SelecionarPorId(pedidoId)).Returns(pedidoExistente);

        EditarQuantidadePedidoContaDto dto = new EditarQuantidadePedidoContaDto(
            Id: pedidoId,
            Quantidade: 0
        );

        // Ação
        Result resultado = servicoPedidoConta.EditarQuantidade(dto);

        // Asserção
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "O campo \"Quantidade\" deve ser um número positivo."));
        repositorioPedidoContaMock.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<PedidoConta>()), Times.Never);
    }

    [TestMethod]
    public void EditarQuantidade_ComQuantidadeValida_DeveAtualizarComSucesso()
    {
        // CT-PED-012 [POSITIVO] — complementa o cenário negativo
        Guid pedidoId = Guid.NewGuid();

        Conta conta = new Conta { UserId = userId, Situacao = SituacaoConta.Aberta };
        Produto produto = new Produto("Cerveja Long Neck", 12.50m) { UserId = userId };
        PedidoConta pedidoExistente = new PedidoConta(produto, 5)
        {
            UserId = userId,
            Conta = conta
        };

        repositorioPedidoContaMock.Setup(r => r.SelecionarPorIdSemFiltro(pedidoId)).Returns(pedidoExistente);
        repositorioPedidoContaMock.Setup(r => r.SelecionarPorId(pedidoId)).Returns(pedidoExistente);
        repositorioPedidoContaMock
            .Setup(r => r.Editar(pedidoId, It.IsAny<PedidoConta>()))
            .Returns(true);

        EditarQuantidadePedidoContaDto dto = new EditarQuantidadePedidoContaDto(
            Id: pedidoId,
            Quantidade: 10
        );

        Result resultado = servicoPedidoConta.EditarQuantidade(dto);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioPedidoContaMock.Verify(r => r.Editar(pedidoId, It.IsAny<PedidoConta>()), Times.Once);
    }

    #endregion
}
