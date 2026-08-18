using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloFaturamento;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloFaturamento;

[TestClass]
public sealed class FaturamentoTests
{
    #region --- CT-FAT-003: Calcular faturamento considerando apenas contas fechadas ---

    [TestMethod]
    public void Calcular_ComListaDeContasFechadas_DeveSomarValorTotalEContarQuantidade()
    {
        // CT-FAT-003 (nível de domínio)
        // Arranjo — ValorTotal é computado a partir de Pedidos, então montamos
        // contas com pedidos reais (mesmo padrão de PedidoContaTests) em vez de
        // atribuir o valor diretamente.
        DateTime data = DateTime.Today;

        Produto produto1 = new Produto("Cerveja Long Neck", 12.50m);
        Produto produto2 = new Produto("Caipirinha", 19.90m);

        // conta1: 100,00 => 2x Cerveja (25,00) + ajuste — usar valores exatos abaixo
        Conta conta1 = new Conta
        {
            Situacao = SituacaoConta.Fechada,
            Pedidos = new List<PedidoConta>
            {
                new PedidoConta(produto1, quantidade: 8) // 8 * 12.50 = 100,00
            }
        };

        Conta conta2 = new Conta
        {
            Situacao = SituacaoConta.Fechada,
            Pedidos = new List<PedidoConta>
            {
                new PedidoConta(produto2, quantidade: 2) // 2 * 19,90 = 39,80
                // ajustar quantidade/produto se quiser fechar em 50,00 redondo
            }
        };

        List<Conta> contasFechadas = new List<Conta> { conta1, conta2 };

        decimal totalEsperado = conta1.ValorTotal + conta2.ValorTotal;

        // Ação
        Faturamento faturamento = Faturamento.Calcular(data, contasFechadas);

        // Asserção
        Assert.AreEqual(totalEsperado, faturamento.ValorTotal);
        Assert.AreEqual(2, faturamento.QuantidadeContasFechadas);
        Assert.AreEqual(data, faturamento.Data);
    }

    [TestMethod]
    public void Calcular_ComListaVazia_DeveRetornarZerado()
    {
        // CT-FAT-003 [complementar] — nenhuma conta fechada no dia
        DateTime data = DateTime.Today;
        List<Conta> contasFechadas = new List<Conta>();

        Faturamento faturamento = Faturamento.Calcular(data, contasFechadas);

        Assert.AreEqual(0m, faturamento.ValorTotal);
        Assert.AreEqual(0, faturamento.QuantidadeContasFechadas);
    }

    #endregion
}
