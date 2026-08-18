using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloFaturamento;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Modulos.ModuloFaturamento;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloFaturamento;

[TestClass]
public sealed class RepositorioFaturamentoEmOrmTests : RepositorioBaseEmOrmTests
{
    private IRepositorioFaturamento repositorioFaturamento = null!;

    [TestInitialize]
    public void InicializarServico()
    {
        ProvedorDeUsuarioFake provedorDeUsuario = new(userId);
        repositorioFaturamento = new RepositorioFaturamentoEmOrm(dbContext, provedorDeUsuario);
    }

    #region --- Helpers de Arranjo ---

    // O repositório filtra por DataAbertura.Date (ver observação no fim).
    // Aqui deixo DataAbertura == DataFechamento de propósito para não expor
    // essa inconsistência dentro do teste.
    private Conta CriarContaFechada(
        DateTime data,
        decimal precoProduto,
        int quantidade,
        Guid? userIdDoBar = null)
    {
        Guid dono = userIdDoBar ?? userId;

        Mesa mesa = new(1, 4) { UserId = dono };
        Garcom garcom = new("Ana") { UserId = dono };

        Conta conta = new(mesa, garcom, "Cliente Teste")
        {
            UserId = dono,
            DataAbertura = data,
            DataFechamento = data,
            Situacao = SituacaoConta.Fechada
        };

        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja Long Neck")
            .With(p => p.PrecoVenda = precoProduto)
            .With(p => p.UserId = dono)
            .Persist();

        dbContext.Set<Mesa>().Add(mesa);
        dbContext.Set<Garcom>().Add(garcom);
        dbContext.Set<Conta>().Add(conta);
        dbContext.SaveChanges();

        PedidoConta pedido = new(produto, quantidade) { Conta = conta, UserId = dono };
        dbContext.Set<PedidoConta>().Add(pedido);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        return conta;
    }

    #endregion

    #region --- CT-FAT-001: Visualizar o faturamento diário do bar com contas fechadas na data ---

    [TestMethod]
    public void ObterFaturamentoDiario_ComContasFechadasNaData_DeveRetornarValorEQuantidadeCorretos()
    {
        // Arranjo
        DateTime hoje = DateTime.Today;

        CriarContaFechada(hoje, precoProduto: 12.50m, quantidade: 4); // 50,00
        CriarContaFechada(hoje, precoProduto: 19.90m, quantidade: 2); // 39,80

        // Ação
        Faturamento faturamento = repositorioFaturamento.ObterFaturamentoDiario(hoje);

        // Asserção
        Assert.AreEqual(2, faturamento.QuantidadeContasFechadas);
        Assert.AreEqual(89.80m, faturamento.ValorTotal);
    }

    #endregion

    #region --- CT-FAT-002: Visualizar o faturamento em uma data sem contas fechadas ---

    [TestMethod]
    public void ObterFaturamentoDiario_SemContasFechadasNaData_DeveRetornarZerado()
    {
        // Arranjo — só existe conta fechada em outro dia
        DateTime hoje = DateTime.Today;
        DateTime ontem = hoje.AddDays(-1);

        CriarContaFechada(ontem, precoProduto: 12.50m, quantidade: 2);

        // Ação
        Faturamento faturamento = repositorioFaturamento.ObterFaturamentoDiario(hoje);

        // Asserção
        Assert.AreEqual(0, faturamento.QuantidadeContasFechadas);
        Assert.AreEqual(0m, faturamento.ValorTotal);
    }

    #endregion

    #region --- CT-FAT-006: Consultar faturamento de uma data sem contas fechadas naquele bar ---

    [TestMethod]
    public void ObterFaturamentoDiario_ComContasFechadasDeOutroBarNaMesmaData_NaoDeveConsiderarNoTotal()
    {
        // Arranjo — conta fechada hoje, mas pertencente a outro bar (outro UserId)
        DateTime hoje = DateTime.Today;
        Guid userIdOutroBar = Guid.NewGuid();

        CriarContaFechada(hoje, precoProduto: 50.00m, quantidade: 1, userIdDoBar: userIdOutroBar);

        // Ação — consulta feita pelo bar autenticado, que não fechou nenhuma conta hoje
        Faturamento faturamento = repositorioFaturamento.ObterFaturamentoDiario(hoje);

        // Asserção
        Assert.AreEqual(0, faturamento.QuantidadeContasFechadas);
        Assert.AreEqual(0m, faturamento.ValorTotal);
    }

    #endregion
}
