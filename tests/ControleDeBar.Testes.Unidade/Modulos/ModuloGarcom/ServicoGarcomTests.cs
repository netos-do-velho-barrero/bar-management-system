using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class ServicoGarcomTests
{
    private Mock<IRepositorioGarcom> repositorioGarcomMock = null!;
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private ServicoGarcom servicoGarcom = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioGarcomMock = new Mock<IRepositorioGarcom>();
        repositorioContaMock = new Mock<IRepositorioConta>();

        servicoGarcom = new ServicoGarcom(
            repositorioGarcomMock.Object,
            repositorioContaMock.Object
        );
    }

    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void SelecionarPorId_VisualizarDadosDeUmGarcom_DeveRetornarDetalhes()
    {
        Guid id = Guid.NewGuid();
        Garcom garcom = new Garcom("Carlos Silva");

        repositorioGarcomMock.Setup(r => r.SelecionarPorId(id)).Returns(garcom);

        Result<DetalhesGarcomDto> resultado = servicoGarcom.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Carlos Silva", resultado.Value.Nome);
    }

    [TestMethod]
    public void SelecionarTodos_ListarTodosOsGarcons_DeveRetornarLista()
    {
        List<Garcom> garcons = [new Garcom("Carlos Silva"), new Garcom("Ana Maria")];
        repositorioGarcomMock.Setup(r => r.SelecionarTodos()).Returns(garcons);

        List<ListarGarcomDto> resultado = servicoGarcom.SelecionarTodos();

        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Excluir_GarcomSemContasAbertas_DeveExcluirComSucesso()
    {
        Guid id = Guid.NewGuid();
        Garcom garcom = new Garcom("Carlos Silva");

        repositorioGarcomMock.Setup(r => r.SelecionarPorId(id)).Returns(garcom);
        repositorioContaMock.Setup(r => r.SelecionarTodos()).Returns([]);
        repositorioGarcomMock.Setup(r => r.Excluir(id)).Returns(true);

        Result resultado = servicoGarcom.Excluir(id);

        Assert.IsTrue(resultado.IsSuccess);
    }

    #endregion
}
