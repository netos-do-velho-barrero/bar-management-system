using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloProduto;

[TestClass]
public sealed class RepositorioProdutoEmOrmTests : RepositorioBaseEmOrmTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Editar_NomeEPrecoDeUmProdutoCadastrado_DeveAtualizarComSucesso()
    {
        // CT-PRO-009: Editar nome e preço de um produto cadastrado [POSITIVO]
        // Arranjo
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck")
            .With(p => p.PrecoVenda = 12.50m)
            .Persist();

        Produto produtoAtualizado = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck 600ml")
            .With(p => p.PrecoVenda = 15.90m)
            .Build();

        // Ação
        bool conseguiuEditar = repositorioProduto.Editar(produto.Id, produtoAtualizado);
        dbContext.ChangeTracker.Clear();

        Produto? produtoSelecionado = repositorioProduto.SelecionarPorId(produto.Id);

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual("Cerveja Long Neck 600ml", produtoSelecionado.Nome);
        Assert.AreEqual(15.90m, produtoSelecionado.PrecoVenda);
    }

    [TestMethod]
    public void Editar_PrecoDeUmProduto_NaoDeveAfetarPedidosJaRegistrados()
    {
        // CT-PRO-010: Editar o preço de um produto sem afetar pedidos já registrados [POSITIVO]
        // Arranjo
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck")
            .With(p => p.PrecoVenda = 12.50m)
            .Persist();

        // Monta a cadeia Mesa -> Garcom -> Conta -> PedidoConta direto no
        // dbContext, já que os repositórios de Mesa/Garcom/Conta ainda não
        // foram mesclados neste projeto de testes (ver RepositorioBaseEmOrmTests).
        Mesa mesa = new(1, 4) { UserId = userId };
        Garcom garcom = new("Ana") { UserId = userId };

        Conta conta = new(mesa, garcom, "Cliente Teste") { UserId = userId };

        dbContext.Set<Mesa>().Add(mesa);
        dbContext.Set<Garcom>().Add(garcom);
        dbContext.Set<Conta>().Add(conta);
        dbContext.SaveChanges();

        PedidoConta pedido = new(produto, 2) { Conta = conta, UserId = userId };

        dbContext.Set<PedidoConta>().Add(pedido);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        decimal precoUnitarioRegistradoAntes = pedido.PrecoUnitario;

        Produto produtoComPrecoAtualizado = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck")
            .With(p => p.PrecoVenda = 25.00m)
            .Build();

        // Ação
        repositorioProduto.Editar(produto.Id, produtoComPrecoAtualizado);
        dbContext.ChangeTracker.Clear();

        PedidoConta? pedidoAposEdicaoDoProduto = dbContext
            .Set<PedidoConta>()
            .SingleOrDefault(p => p.Id == pedido.Id);

        // Asserção
        Assert.IsNotNull(pedidoAposEdicaoDoProduto);
        Assert.AreEqual(precoUnitarioRegistradoAntes, pedidoAposEdicaoDoProduto.PrecoUnitario);
        Assert.AreEqual(12.50m, pedidoAposEdicaoDoProduto.PrecoUnitario);
        Assert.AreNotEqual(25.00m, pedidoAposEdicaoDoProduto.PrecoUnitario);
    }

    #endregion
}
