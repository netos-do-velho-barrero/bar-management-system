using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class MesaTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Validar_ComNumeroEQuantidadeDeLugaresValidos_DevePassarSemErros()
    {
        // CT-MES-001: Cadastrar mesa com número e quantidade de lugares válidos
        Mesa mesa = new Mesa(numero: 1, quantidadeLugares: 4);

        List<string> erros = mesa.Validar();

        Assert.AreEqual(0, erros.Count);
        Assert.AreEqual(StatusMesa.Livre, mesa.Status);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void Validar_SemNumero_DeveRetornarErro()
    {
        // CT-MES-002: Cadastrar mesa sem informar o número (default/0)
        Mesa mesa = new Mesa(numero: 0, quantidadeLugares: 4);

        List<string> erros = mesa.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Número\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_SemQuantidadeDeLugares_DeveRetornarErro()
    {
        // CT-MES-003: Cadastrar mesa sem informar a quantidade de lugares
        Mesa mesa = new Mesa(numero: 1, quantidadeLugares: default);

        List<string> erros = mesa.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade de Lugares\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_ComQuantidadeLugaresZero_DeveRetornarErro()
    {
        // CT-MES-004: Cadastrar mesa com quantidade de lugares igual a zero
        Mesa mesa = new Mesa(numero: 1, quantidadeLugares: 0);

        List<string> erros = mesa.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade de Lugares\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_ComQuantidadeLugaresNegativa_DeveRetornarErro()
    {
        // CT-MES-005: Cadastrar mesa com quantidade de lugares negativa
        Mesa mesa = new Mesa(numero: 1, quantidadeLugares: -2);

        List<string> erros = mesa.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade de Lugares\" deve ser maior que zero.", erros.First());
    }

    #endregion
}
