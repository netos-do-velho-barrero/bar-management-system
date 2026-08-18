using ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;
using FluentResults;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloPedidoConta;

[TestClass]
public sealed class RepositorioPedidoContaEmOrmTests : RepositorioBaseEmOrmTests
{
    // repositorioPedidoConta já vem herdado e inicializado pela classe base
    // (RepositorioBaseEmOrmTests.InicializarContexto) — não redeclarar aqui.
    private RepositorioContaEmOrm repositorioConta = null!;
    private ServicoPedidoConta servicoPedidoConta = null!;

    [TestInitialize]
    public void InicializarServico()
    {
        ProvedorDeUsuarioFake provedorDeUsuario = new(userId);

        repositorioConta = new RepositorioContaEmOrm(dbContext, provedorDeUsuario);

        servicoPedidoConta = new ServicoPedidoConta(
            repositorioPedidoConta,
            repositorioConta,
            repositorioProduto,
            provedorDeUsuario
        );
    }

    #region --- Helpers de Arranjo ---

    private Conta CriarContaAberta()
    {
        Mesa mesa = new(1, 4) { UserId = userId };
        Garcom garcom = new("Ana") { UserId = userId };
        Conta conta = new(mesa, garcom, "Cliente Teste") { UserId = userId };

        dbContext.Set<Mesa>().Add(mesa);
        dbContext.Set<Garcom>().Add(garcom);
        dbContext.Set<Conta>().Add(conta);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        return conta;
    }

    private Produto CriarProduto(string nome = "Cerveja Long Neck", decimal preco = 12.50m)
    {
        return Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = nome)
            .With(p => p.PrecoVenda = preco)
            .Persist();
    }

    #endregion

    #region --- CT-PED-001: Adicionar produto a uma conta aberta ---

    [TestMethod]
    public void Adicionar_ProdutoAUmaContaAberta_DevePersistirComSucesso()
    {
        // CT-PED-001 [POSITIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto();

        AdicionarPedidoContaDto dto = new(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 3
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsSuccess);

        PedidoConta? pedidoPersistido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .SingleOrDefault();

        Assert.IsNotNull(pedidoPersistido);
        Assert.AreEqual(produto.Id, pedidoPersistido.ProdutoId);
        Assert.AreEqual(3, pedidoPersistido.Quantidade);
        Assert.AreEqual(12.50m, pedidoPersistido.PrecoUnitario);
    }

    #endregion

    #region --- CT-PED-006: Adicionar produto pertencente a outro bar ---

    [TestMethod]
    public void Adicionar_ProdutoPertencenteAOutroBar_DeveRetornarFalha()
    {
        // CT-PED-006 [NEGATIVO]
        Conta conta = CriarContaAberta();

        Guid outroUserId = Guid.NewGuid();
        Produto produtoDeOutroBar = new("Cerveja Importada", 20.00m) { UserId = outroUserId };

        dbContext.Set<Produto>().Add(produtoDeOutroBar);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        AdicionarPedidoContaDto dto = new(
            ContaId: conta.Id,
            ProdutoId: produtoDeOutroBar.Id,
            Quantidade: 1
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "Este produto não pertence ao seu bar."));
        Assert.IsEmpty(repositorioPedidoConta.Filtrar(p => p.ContaId == conta.Id));
    }

    #endregion

    #region --- CT-PED-007: Adicionar produto a uma conta fechada ---

    [TestMethod]
    public void Adicionar_ProdutoAUmaContaFechada_DeveRetornarFalha()
    {
        // CT-PED-007 [NEGATIVO]
        Conta conta = CriarContaAberta();

        repositorioConta.AlterarSituacao(conta.Id, SituacaoConta.Fechada);
        dbContext.ChangeTracker.Clear();

        Produto produto = CriarProduto();

        AdicionarPedidoContaDto dto = new(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 1
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "Não é possível adicionar pedidos a uma conta fechada."));
        Assert.IsEmpty(repositorioPedidoConta.Filtrar(p => p.ContaId == conta.Id));
    }

    #endregion

    #region --- CT-PED-008: Registrar o preço do produto no momento da inclusão do pedido ---

    [TestMethod]
    public void Adicionar_DeveRegistrarPrecoDoProdutoNoMomentoDaInclusao()
    {
        // CT-PED-008 [POSITIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto(preco: 18.75m);

        AdicionarPedidoContaDto dto = new(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 2
        );

        Result resultado = servicoPedidoConta.Adicionar(dto);
        dbContext.ChangeTracker.Clear();

        PedidoConta? pedidoPersistido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .SingleOrDefault();

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(pedidoPersistido);
        Assert.AreEqual(18.75m, pedidoPersistido.PrecoUnitario);
    }

    #endregion

    #region --- CT-PED-009: Alterar o preço de um produto não deve afetar pedidos já registrados ---

    [TestMethod]
    public void EditarPrecoDoProduto_NaoDeveAfetarPedidosJaRegistrados()
    {
        // CT-PED-009 [POSITIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto(preco: 12.50m);

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 2
        ));
        dbContext.ChangeTracker.Clear();

        Produto produtoComPrecoAtualizado = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck")
            .With(p => p.PrecoVenda = 25.00m)
            .Build();

        repositorioProduto.Editar(produto.Id, produtoComPrecoAtualizado);
        dbContext.ChangeTracker.Clear();

        PedidoConta? pedidoAposEdicaoDoProduto = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .SingleOrDefault();

        Assert.IsNotNull(pedidoAposEdicaoDoProduto);
        Assert.AreEqual(12.50m, pedidoAposEdicaoDoProduto.PrecoUnitario);
        Assert.AreNotEqual(25.00m, pedidoAposEdicaoDoProduto.PrecoUnitario);
    }

    #endregion

    #region --- CT-PED-011: Alterar a quantidade de um produto consumido ---

    [TestMethod]
    public void EditarQuantidade_DeUmProdutoConsumido_DeveAtualizarComSucesso()
    {
        // CT-PED-011 [POSITIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto();

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 2
        ));
        dbContext.ChangeTracker.Clear();

        PedidoConta pedido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .Single();

        Result resultado = servicoPedidoConta.EditarQuantidade(
            new EditarQuantidadePedidoContaDto(Id: pedido.Id, Quantidade: 7)
        );
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsSuccess);

        PedidoConta? pedidoAtualizado = repositorioPedidoConta.SelecionarPorId(pedido.Id);

        Assert.IsNotNull(pedidoAtualizado);
        Assert.AreEqual(7, pedidoAtualizado.Quantidade);
    }

    #endregion

    #region --- CT-PED-013: Alterar um pedido de uma conta fechada ---

    [TestMethod]
    public void EditarQuantidade_DeUmPedidoDeContaFechada_DeveRetornarFalha()
    {
        // CT-PED-013 [NEGATIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto();

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 2
        ));
        dbContext.ChangeTracker.Clear();

        PedidoConta pedido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .Single();

        repositorioConta.AlterarSituacao(conta.Id, SituacaoConta.Fechada);
        dbContext.ChangeTracker.Clear();

        Result resultado = servicoPedidoConta.EditarQuantidade(
            new EditarQuantidadePedidoContaDto(Id: pedido.Id, Quantidade: 5)
        );
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "Não é possível alterar pedidos de uma conta fechada."));

        PedidoConta? pedidoInalterado = repositorioPedidoConta.SelecionarPorId(pedido.Id);
        Assert.AreEqual(2, pedidoInalterado!.Quantidade);
    }

    #endregion

    #region --- CT-PED-014: Remover produto de uma conta aberta ---

    [TestMethod]
    public void Remover_ProdutoDeUmaContaAberta_DeveExcluirComSucesso()
    {
        // CT-PED-014 [POSITIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto();

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 1
        ));
        dbContext.ChangeTracker.Clear();

        PedidoConta pedido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .Single();

        Result resultado = servicoPedidoConta.Remover(pedido.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsSuccess);

        PedidoConta? pedidoRemovido = repositorioPedidoConta.SelecionarPorId(pedido.Id);

        Assert.IsNull(pedidoRemovido);
        Assert.IsEmpty(repositorioPedidoConta.Filtrar(p => p.ContaId == conta.Id));
    }

    #endregion

    #region --- CT-PED-015: Remover produto de uma conta fechada ---

    [TestMethod]
    public void Remover_ProdutoDeUmaContaFechada_DeveRetornarFalha()
    {
        // CT-PED-015 [NEGATIVO]
        Conta conta = CriarContaAberta();
        Produto produto = CriarProduto();

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: produto.Id,
            Quantidade: 1
        ));
        dbContext.ChangeTracker.Clear();

        PedidoConta pedido = repositorioPedidoConta
            .Filtrar(p => p.ContaId == conta.Id)
            .Single();

        repositorioConta.AlterarSituacao(conta.Id, SituacaoConta.Fechada);
        dbContext.ChangeTracker.Clear();

        Result resultado = servicoPedidoConta.Remover(pedido.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(resultado.IsFailed);
        Assert.IsTrue(resultado.Errors.Any(e => e.Message == "Não é possível remover pedidos de uma conta fechada."));

        PedidoConta? pedidoAindaExiste = repositorioPedidoConta.SelecionarPorId(pedido.Id);
        Assert.IsNotNull(pedidoAindaExiste);
    }

    #endregion

    #region --- CT-PED-016: Visualizar todos os produtos consumidos em uma conta ---

    [TestMethod]
    public void SelecionarPorConta_DeveListarTodosOsProdutosConsumidos()
    {
        // CT-PED-016 [POSITIVO]
        Conta conta = CriarContaAberta();

        Produto cerveja = CriarProduto("Cerveja Long Neck", 12.50m);
        Produto caipirinha = CriarProduto("Caipirinha", 19.90m);

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: cerveja.Id,
            Quantidade: 2
        ));

        servicoPedidoConta.Adicionar(new AdicionarPedidoContaDto(
            ContaId: conta.Id,
            ProdutoId: caipirinha.Id,
            Quantidade: 1
        ));

        dbContext.ChangeTracker.Clear();

        List<ListarPedidoContaDto> resultado = servicoPedidoConta.SelecionarPorConta(conta.Id);

        Assert.HasCount(2, resultado);

        Assert.IsTrue(resultado.Any(p =>
            p.NomeProduto == "Cerveja Long Neck" &&
            p.Quantidade == 2 &&
            p.PrecoUnitario == 12.50m &&
            p.Subtotal == 25.00m
        ));

        Assert.IsTrue(resultado.Any(p =>
            p.NomeProduto == "Caipirinha" &&
            p.Quantidade == 1 &&
            p.PrecoUnitario == 19.90m &&
            p.Subtotal == 19.90m
        ));
    }

    #endregion
}
