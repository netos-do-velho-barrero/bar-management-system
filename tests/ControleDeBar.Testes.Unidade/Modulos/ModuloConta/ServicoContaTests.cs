using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ServicoContaTests
{
    private Mock<IRepositorioConta> repositorioContaMock = null!;
    private Mock<IRepositorioMesa> repositorioMesaMock = null!;
    private Mock<IRepositorioGarcom> repositorioGarcomMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private ServicoConta servicoConta = null!;
    private readonly Guid usuarioId = Guid.NewGuid();

    [TestInitialize]
    public void Setup()
    {
        repositorioContaMock = new Mock<IRepositorioConta>();
        repositorioMesaMock = new Mock<IRepositorioMesa>();
        repositorioGarcomMock = new Mock<IRepositorioGarcom>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        provedorDeUsuarioMock.Setup(p => p.Id).Returns(usuarioId);

        servicoConta = new ServicoConta(
            repositorioContaMock.Object,
            repositorioMesaMock.Object,
            repositorioGarcomMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void AbrirConta_MesaLivre_DeveAbrirComSucesso()
    {
        Guid mesaId = Guid.NewGuid();
        Guid garcomId = Guid.NewGuid();

        Mesa mesa = new Mesa(1, 4) { Status = StatusMesa.Livre, UserId = usuarioId };
        Garcom garcom = new Garcom("Carlos") { UserId = usuarioId };

        repositorioMesaMock.Setup(r => r.SelecionarPorIdSemFiltro(mesaId)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesaId)).Returns(mesa);
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcomId)).Returns(garcom);

        AbrirContaDto dto = new AbrirContaDto(mesaId, garcomId, "Cliente Teste");
        Result resultado = servicoConta.Abrir(dto);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioMesaMock.Verify(r => r.AlterarStatus(mesaId, StatusMesa.Ocupada), Times.Once);
    }

    [TestMethod]
    public void FecharConta_ContaAberta_DeveAlterarSituacaoEMesaVoltarParaLivre()
    {
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4) { Status = StatusMesa.Ocupada, UserId = usuarioId };
        Garcom garcom = new Garcom("Carlos") { UserId = usuarioId };
        Conta conta = new Conta(mesa, garcom, "Cliente Teste") { UserId = usuarioId, Situacao = SituacaoConta.Aberta };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(id)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(id)).Returns(conta);

        Result resultado = servicoConta.Fechar(id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        repositorioMesaMock.Verify(r => r.AlterarStatus(conta.MesaId, StatusMesa.Livre), Times.Once);
    }

    #endregion

    #region --- CENÁRIOS NEGATIVOS ---

    [TestMethod]
    public void AbrirConta_ParaMesaComContaAberta_DeveRetornarFalha()
    {
        Guid mesaId = Guid.NewGuid();
        Guid garcomId = Guid.NewGuid();

        Mesa mesa = new Mesa(1, 4) { Status = StatusMesa.Ocupada, UserId = usuarioId };
        Garcom garcom = new Garcom("Carlos") { UserId = usuarioId };

        repositorioMesaMock.Setup(r => r.SelecionarPorIdSemFiltro(mesaId)).Returns(mesa);
        repositorioMesaMock.Setup(r => r.SelecionarPorId(mesaId)).Returns(mesa);
        repositorioGarcomMock.Setup(r => r.SelecionarPorId(garcomId)).Returns(garcom);

        AbrirContaDto dto = new AbrirContaDto(mesaId, garcomId, "Outro Cliente");
        Result resultado = servicoConta.Abrir(dto);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void FecharConta_ContaJaFechada_DeveRetornarFalha()
    {
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4) { UserId = usuarioId };
        Garcom garcom = new Garcom("Carlos") { UserId = usuarioId };
        Conta conta = new Conta(mesa, garcom, "Cliente Teste") { UserId = usuarioId, Situacao = SituacaoConta.Fechada };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(id)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(id)).Returns(conta);

        Result resultado = servicoConta.Fechar(id);

        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_DadosDeContaFechada_DeveRetornarFalha()
    {
        Guid id = Guid.NewGuid();
        Mesa mesa = new Mesa(1, 4) { UserId = usuarioId };
        Garcom garcom = new Garcom("Carlos") { UserId = usuarioId };
        Conta conta = new Conta(mesa, garcom, "Cliente Teste") { UserId = usuarioId, Situacao = SituacaoConta.Fechada };

        repositorioContaMock.Setup(r => r.SelecionarPorIdSemFiltro(id)).Returns(conta);
        repositorioContaMock.Setup(r => r.SelecionarPorId(id)).Returns(conta);

        EditarContaDto dto = new EditarContaDto(id, mesa.Id, garcom.Id, "Novo Nome Cliente");
        Result resultado = servicoConta.Editar(dto);

        Assert.IsTrue(resultado.IsFailed);
    }

    #endregion
}
