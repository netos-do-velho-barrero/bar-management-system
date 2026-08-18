using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloGarcom;

[TestClass]
public sealed class RepositorioGarcomEmOrmTests : RepositorioBaseEmOrmTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Editar_NomeGarcom_DeveAtualizarComSucesso()
    {
        Garcom garcom = new Garcom("Ana") { UserId = userId };
        repositorioGarcom.Cadastrar(garcom);
        dbContext.SaveChanges();

        Garcom garcomAtualizado = new Garcom("Ana Paula") { UserId = userId };

        bool conseguiuEditar = repositorioGarcom.Editar(garcom.Id, garcomAtualizado);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        Garcom? garcomSelecionado = repositorioGarcom.SelecionarPorId(garcom.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(garcomSelecionado);
        Assert.AreEqual("Ana Paula", garcomSelecionado.Nome);
    }

    #endregion
}
