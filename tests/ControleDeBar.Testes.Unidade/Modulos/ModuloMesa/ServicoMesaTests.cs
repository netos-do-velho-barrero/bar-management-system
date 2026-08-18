using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class ServicoMesaTests
{
    private Mock<IRepositorioMesa> repositorioMesaMock = null!;
    private ServicoMesa servicoMesa = null!;

    [TestInitialize]
    public void Setup()
    {
        repositorioMesaMock = new Mock<IRepositorioMesa>();
        servicoMesa = new ServicoMesa(repositorioMesaMock.Object);
    }

    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void SelecionarPorId_VisualizarDadosDeUmaMesa_DeveRetornarDetalhes()
    {
        // CT-MES-010 [POSITIVO]
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4);

        repositorioMesaMock.Setup(r => r.SelecionarPorId(id)).Returns(mesa);

        Result<DetalhesMesaDto> resultado = servicoMesa.SelecionarPorId(id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(1, resultado.Value.Numero);
        Assert.AreEqual(4, resultado.Value.QuantidadeLugares);
    }

    [TestMethod]
    public void SelecionarTodos_ListarTodasAsMesasDoBar_DeveRetornarLista()
    {
        // CT-MES-011 [POSITIVO]
        List<Mesa> mesas = [new Mesa(1, 4), new Mesa(2, 2)];
        repositorioMesaMock.Setup(r => r.SelecionarTodos()).Returns(mesas);

        List<ListarMesaDto> resultado = servicoMesa.SelecionarTodos();

        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void SelecionarTodos_FiltrarMesasOcupadas_DeveRetornarApenasOcupadas()
    {
        // CT-MES-012 [POSITIVO]
        Mesa mesa1 = new Mesa(1, 4) { Status = StatusMesa.Ocupada };
        Mesa mesa2 = new Mesa(2, 2) { Status = StatusMesa.Livre };

        repositorioMesaMock.Setup(r => r.SelecionarTodos()).Returns([mesa1, mesa2]);

        List<ListarMesaDto> resultado = servicoMesa.SelecionarTodos()
            .Where(m => m.Status == StatusMesa.Ocupada.ToString())
            .ToList();

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(1, resultado.First().Numero);
    }

    [TestMethod]
    public void Excluir_MesaLivreSemContaVinculada_DeveExcluirComSucesso()
    {
        // CT-MES-013 [POSITIVO]
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4) { Status = StatusMesa.Livre };

        repositorioMesaMock.Setup(r => r.SelecionarPorId(id)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.Excluir(id)).Returns(true);

        Result resultado = servicoMesa.Excluir(id);

        Assert.IsTrue(resultado.IsSuccess);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void Excluir_MesaComContaAbertaVinculada_DeveRetornarFalha()
    {
        // CT-MES-014 [NEGATIVO]
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4) { Status = StatusMesa.Ocupada };

        repositorioMesaMock.Setup(r => r.SelecionarPorId(id)).Returns(mesa);

        Result resultado = servicoMesa.Excluir(id);

        Assert.IsTrue(resultado.IsFailed);
    }

    #endregion
}
