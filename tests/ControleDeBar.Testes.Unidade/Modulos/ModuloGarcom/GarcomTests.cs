using ControleDeBar.Dominio.Modulos.ModuloGarcom;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Validar_ComNomeValido_DevePassarSemErros()
    {
        // CT-GAR-001: Cadastrar garçom com nome válido
        Garcom garcom = new Garcom(nome: "Carlos Silva");

        List<string> erros = garcom.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_ComNomeNoLimiteMinimo_DevePassarSemErros()
    {
        // CT-GAR-004: Cadastrar garçom com nome no limite mínimo (2 caracteres)
        Garcom garcom = new Garcom(nome: "Zé");

        List<string> erros = garcom.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_ComNomeNoLimiteMaximo_DevePassarSemErros()
    {
        // CT-GAR-005: Cadastrar garçom com nome no limite máximo (100 caracteres)
        Garcom garcom = new Garcom(nome: new string('A', 100));

        List<string> erros = garcom.Validar();

        Assert.AreEqual(0, erros.Count);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void Validar_SemNome_DeveRetornarErro()
    {
        // CT-GAR-002: Cadastrar garçom sem informar o nome
        Garcom garcom = new Garcom(nome: string.Empty);

        List<string> erros = garcom.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    [TestMethod]
    public void Validar_ComNomeAbaixoDoMinimo_DeveRetornarErro()
    {
        // CT-GAR-003: Cadastrar garçom com nome abaixo do mínimo (1 caractere)
        Garcom garcom = new Garcom(nome: "A");

        List<string> erros = garcom.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    [TestMethod]
    public void Validar_ComNomeAcimaDoMaximo_DeveRetornarErro()
    {
        // CT-GAR-006: Cadastrar garçom com nome acima do máximo (101 caracteres)
        Garcom garcom = new Garcom(nome: new string('A', 101));

        List<string> erros = garcom.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    #endregion
}
