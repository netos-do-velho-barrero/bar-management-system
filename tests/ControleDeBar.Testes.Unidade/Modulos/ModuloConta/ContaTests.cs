using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ContaTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Validar_ComDadosValidos_DevePassarSemErros()
    {
        // CT-CTA-001: Abrir conta com mesa, garçom e cliente válidos
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, "Cliente Teste");

        List<string> erros = conta.Validar();

        Assert.AreEqual(0, erros.Count);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
    }

    [TestMethod]
    public void Validar_ComNomeClienteNoLimiteMinimo_DevePassarSemErros()
    {
        // CT-CTA-006: Abrir conta com nome do cliente no limite mínimo (2 caracteres)
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, "Lu");

        List<string> erros = conta.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_ComNomeClienteNoLimiteMaximo_DevePassarSemErros()
    {
        // CT-CTA-007: Abrir conta com nome do cliente no limite máximo (100 caracteres)
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, new string('A', 100));

        List<string> erros = conta.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Construtor_DeveRegistrarDataDeAberturaAutomaticamente()
    {
        // CT-CTA-011: Registrar automaticamente a data de abertura da conta
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        DateTime dataAntes = DateTime.Now;

        Conta conta = new Conta(mesa, garcom, "Cliente Teste");

        Assert.IsTrue(conta.DataAbertura >= dataAntes && conta.DataAbertura <= DateTime.Now);
    }

    [TestMethod]
    public void CalcularValorTotal_ComPedidosVinculados_DeveSomarCorretamente()
    {
        // CT-CTA-016: Calcular o valor total da conta com base nos pedidos
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, "Cliente Teste");

        Produto p1 = new Produto("Cerveja", 10.00m);
        Produto p2 = new Produto("Porção", 25.50m);

        conta.Pedidos.Add(new PedidoConta(p1, 2) { Conta = conta }); // 20.00
        conta.Pedidos.Add(new PedidoConta(p2, 1) { Conta = conta }); // 25.50

        Assert.AreEqual(45.50m, conta.ValorTotal);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void Validar_SemMesa_DeveRetornarErro()
    {
        // CT-CTA-002: Abrir conta sem selecionar a mesa
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(null!, garcom, "Cliente Teste");

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Contains("A mesa deve ser informada."));
    }

    [TestMethod]
    public void Validar_SemGarcom_DeveRetornarErro()
    {
        // CT-CTA-003: Abrir conta sem selecionar o garçom
        Mesa mesa = new Mesa(1, 4);
        Conta conta = new Conta(mesa, null!, "Cliente Teste");

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Contains("O garçom deve ser informado."));
    }

    [TestMethod]
    public void Validar_SemNomeCliente_DeveRetornarErro()
    {
        // CT-CTA-004: Abrir conta sem informar o nome do cliente
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, string.Empty);

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Contains("O campo \"Cliente\" deve conter entre 2 e 100 caracteres."));
    }

    [TestMethod]
    public void Validar_ComNomeClienteAbaixoDoMinimo_DeveRetornarErro()
    {
        // CT-CTA-005: Abrir conta com nome do cliente abaixo do mínimo
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, "A");

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Contains("O campo \"Cliente\" deve conter entre 2 e 100 caracteres."));
    }

    [TestMethod]
    public void Validar_ComNomeClienteAcimaDoMaximo_DeveRetornarErro()
    {
        // CT-CTA-008: Abrir conta com nome do cliente acima do máximo
        Mesa mesa = new Mesa(1, 4);
        Garcom garcom = new Garcom("João");
        Conta conta = new Conta(mesa, garcom, new string('A', 101));

        List<string> erros = conta.Validar();

        Assert.IsTrue(erros.Contains("O campo \"Cliente\" deve conter entre 2 e 100 caracteres."));
    }

    #endregion
}
