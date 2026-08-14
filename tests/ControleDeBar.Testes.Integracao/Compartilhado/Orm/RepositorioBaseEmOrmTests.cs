using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected ControleDeBarDbContext dbContext = null!;

    // Hooks / Ganchos
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static ControleDeBarDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase($"integracao-{Guid.NewGuid():N}")
                .Options;

        return new ControleDeBarDbContext(options, new ProvedorDeUsuarioFake(userId));
    }
}
