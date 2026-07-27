using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.Infra.Modulos.ModuloDisciplina;
using GeradorDeProvas.Infra.Modulos.ModuloProva;
using GeradorDeProvas.Testes.Integracao.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;
using FizzWare.NBuilder;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;
using GeradorDeProvas.Infra.Modulos.ModuloMateria;
using GeradorDeProvas.Dominio.Modulos.ModuloMateria;

namespace GeradorDeProvas.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected GeradorDeProvasDbContext dbContext = null!;
    protected RepositorioDisciplinaEmOrm repositorioDisciplina = null!;
    protected RepositorioMateriaEmOrm repositorioMateria = null!;
    protected RepositorioProvaEmOrm repositorioProva = null!;

    // Hooks / Ganchos
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        repositorioProva = new RepositorioProvaEmOrm(dbContext);

        // Disciplina
        repositorioDisciplina = new RepositorioDisciplinaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Disciplina>(repositorioDisciplina.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Disciplina>>((disciplinas) =>
        {
            foreach (Disciplina d in disciplinas)
                repositorioDisciplina.Cadastrar(d);
        });

        // Materia
        repositorioMateria = new RepositorioMateriaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Materia>(repositorioMateria.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Materia>>((materias) =>
        {
            foreach (Materia m in materias)
                repositorioMateria.Cadastrar(m);
        });
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static GeradorDeProvasDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<GeradorDeProvasDbContext> options =
            new DbContextOptionsBuilder<GeradorDeProvasDbContext>()
                .UseInMemoryDatabase("GeradorDeProvasTestDB_Memory")
                .Options;

        return new GeradorDeProvasDbContext(options, new ProvedorDeUsuarioFake(userId));
    }
}
