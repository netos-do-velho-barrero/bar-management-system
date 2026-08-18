using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloMesa;

[TestClass]
public sealed class RepositorioMesaEmOrmTests : RepositorioBaseEmOrmTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Editar_QuantidadeDeLugares_DeveAtualizarComSucesso()
    {
        Mesa mesa = new Mesa(5, 4) { UserId = userId };
        repositorioMesa.Cadastrar(mesa);
        dbContext.SaveChanges();

        Mesa mesaAtualizada = new Mesa(5, 8) { UserId = userId };

        bool conseguiuEditar = repositorioMesa.Editar(mesa.Id, mesaAtualizada);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        Mesa? mesaSelecionada = repositorioMesa.SelecionarPorId(mesa.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(mesaSelecionada);
        Assert.AreEqual(8, mesaSelecionada.QuantidadeLugares);
    }

    [TestMethod]
    public void Cadastrar_MesaComMesmoNumeroEmBaresDiferentes_DevePermitir()
    {
        Guid outroUserId = Guid.NewGuid();

        Mesa mesaBar1 = new Mesa(10, 4) { UserId = userId };
        Mesa mesaBar2 = new Mesa(10, 6) { UserId = outroUserId };

        repositorioMesa.Cadastrar(mesaBar1);
        dbContext.SaveChanges();

        dbContext.Set<Mesa>().Add(mesaBar2);
        dbContext.SaveChanges();

        Mesa? mesaDoBar1 = repositorioMesa.SelecionarPorId(mesaBar1.Id);
        Assert.IsNotNull(mesaDoBar1);
        Assert.AreEqual(4, mesaDoBar1.QuantidadeLugares);
    }

    #endregion
}
