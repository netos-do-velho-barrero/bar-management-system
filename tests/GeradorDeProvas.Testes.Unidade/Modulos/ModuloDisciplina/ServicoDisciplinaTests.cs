using FluentResults;
using GeradorDeProvas.Aplicacao.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Compartilhado;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;
using Moq;

namespace GeradorDeProvas.Testes.Unidade.Modulos.ModuloDisciplina;

[TestClass]
public sealed class ServicoDisciplinaTests
{
    [TestMethod]
    public void Cadastrar_DadosValidos_PersisteDisciplina()
    {
        // Arrange
        Mock<IRepositorioDisciplina> repositorioDisciplina = new Mock<IRepositorioDisciplina>();
        Mock<IRepositorioMateria> repositorioMateria = new Mock<IRepositorioMateria>();

        repositorioDisciplina.Setup(r => r.SelecionarTodos()).Returns([]);

        Disciplina? disciplinaCadastrada = null;

        repositorioDisciplina
            .Setup(r => r.Cadastrar(It.IsAny<Disciplina>()))
            .Callback<Disciplina>(
                disciplina => disciplinaCadastrada = disciplina
            );

        ServicoDisciplina servicoDisciplina = new ServicoDisciplina(
            repositorioDisciplina.Object,
            repositorioMateria.Object
        );

        // Act
        Result resultado = servicoDisciplina.Cadastrar(new CadastrarDisciplinaDto("Matemática"));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(disciplinaCadastrada);
        Assert.AreEqual("Matemática", disciplinaCadastrada.Nome);

        repositorioDisciplina.Verify(r => r.Cadastrar(It.IsAny<Disciplina>()), Times.Once);
    }
}
