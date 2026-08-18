using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloFaturamento;

[TestClass]
public sealed class ServicoFaturamentoTests
{
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private ServicoFaturamento servicoFaturamento = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioContaMock = new Mock<IRepositorioConta>();
        servicoFaturamento = new ServicoFaturamento(repositorioContaMock.Object);
    }

    #region --- CT-FAT-003: Calcular faturamento considerando apenas contas fechadas ---

    [TestMethod]
    public void ObterFaturamentoDiario_ComContasAbertasEFechadas_DeveConsiderarApenasAsFechadas()
    {
        // CT-FAT-003 (nível de aplicação)
        // Arranjo
        DateTime hoje = DateTime.Today;

        Conta contaFechadaDoDia = new Conta
        {
            Situacao = SituacaoConta.Fechada,
            DataAbertura = hoje,
            DataFechamento = hoje
        };

        Conta contaAbertaDoDia = new Conta
        {
            Situacao = SituacaoConta.Aberta,
            DataAbertura = hoje,
            DataFechamento = null
        };

        Conta contaFechadaDeOutroDia = new Conta
        {
            Situacao = SituacaoConta.Fechada,
            DataAbertura = hoje.AddDays(-1),
            DataFechamento = hoje.AddDays(-1)
        };

        repositorioContaMock
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Conta> { contaFechadaDoDia, contaAbertaDoDia, contaFechadaDeOutroDia });

        // Ação
        FaturamentoDto resultado = servicoFaturamento.ObterFaturamentoDiario(hoje);

        // Asserção
        Assert.AreEqual(1, resultado.QuantidadeContas);
        Assert.AreEqual(1, resultado.ContasFechadas.Count);
        Assert.AreEqual(contaFechadaDoDia.Id, resultado.ContasFechadas.First().Id);
    }

    [TestMethod]
    public void ObterFaturamentoDiario_SemContasFechadasNoDia_DeveRetornarZerado()
    {
        // CT-FAT-003 [complementar]
        DateTime hoje = DateTime.Today;

        repositorioContaMock
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Conta>());

        FaturamentoDto resultado = servicoFaturamento.ObterFaturamentoDiario(hoje);

        Assert.AreEqual(0, resultado.QuantidadeContas);
        Assert.AreEqual(0m, resultado.ValorTotal);
        Assert.AreEqual(0, resultado.ContasFechadas.Count);
    }

    #endregion
}
