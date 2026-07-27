using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;

namespace GeradorDeProvas.Testes.Integracao.ModuloMateria;

[TestClass]
public sealed class RepositorioMateriaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void CadastrarESelecionarPorId_CarregaRegistro_ComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Nome1");

        Materia materia = new Materia("Nome1", 1, disciplina);

        // Ação
        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsNotNull(materiaSelecionada);
        Assert.AreEqual("Nome1", materiaSelecionada.Nome);
    }

    [TestMethod]
    public void Editar_AtualizaRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Nome1");
        Materia materia = new Materia("Nome1", 1, disciplina);

        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        Disciplina disciplinaAtualizada = new Disciplina("Nome2");
        Materia materiaAtualizada = new Materia("NomeAtualizado", 2, disciplinaAtualizada);

        // Ação
        bool conseguiuEditar = repositorioMateria.Editar(materia.Id, materiaAtualizada);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(materiaSelecionada);
        Assert.AreEqual("NomeAtualizado", materiaSelecionada.Nome);
        Assert.AreEqual(2, materiaSelecionada.Serie);
        Assert.AreEqual(disciplinaAtualizada.Nome, materiaSelecionada.Disciplina.Nome);
    }

    [TestMethod]
    public void Excluir_RemoveRegistroExistente()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Nome1");
        Materia materia = new Materia("Nome1", 1, disciplina);

        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        // Ação
        bool conseguiuExcluir = repositorioMateria.Excluir(materia.Id);
        dbContext.ChangeTracker.Clear();

        Materia? materiaSelecionada = repositorioMateria.SelecionarPorId(materia.Id);

        // Asserção
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(materiaSelecionada);
    }

    [TestMethod]
    public void SelecionarTodos_CarregaRegistros_ComRelacionamentos()
    {
        // Arranjo
        Disciplina disciplina = new Disciplina("Nome1");
        Materia materia = new Materia("Nome1", 1, disciplina);

        repositorioMateria.Cadastrar(materia);
        dbContext.ChangeTracker.Clear();

        // Ação
        List<Materia> materias = repositorioMateria.SelecionarTodos();

        // Asserção
        Assert.HasCount(1, materias);
        Assert.AreEqual("Nome1", materias.First().Nome);
        Assert.AreEqual(1, materias.First().Serie);
        Assert.AreEqual("Nome1", materias.First().Disciplina.Nome);
    }
}
