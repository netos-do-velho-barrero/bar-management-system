using Microsoft.EntityFrameworkCore;
using FizzWare.NBuilder;

using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;

using ControleDeBar.Infra.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

using ControleDeBar.Infra.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected ControleDeBarDbContext dbContext = null!;

    // UserId fixo por execução de teste — usado para simular o usuário
    // autenticado (multi-tenancy) e reaproveitado pelos repositórios abaixo.
    protected Guid userId;

    protected RepositorioProdutoEmOrm repositorioProduto = null!;

    protected RepositorioMesaEmOrm repositorioMesa = null!;
    protected RepositorioGarcomEmOrm repositorioGarcom = null!;
    protected RepositorioContaEmOrm repositorioConta = null!;
    protected RepositorioPedidoContaEmOrm repositorioPedidoConta = null!;

    // Hooks / Ganchos
    [TestInitialize]
    public void InicializarContexto()
    {
        userId = Guid.NewGuid();
        dbContext = CriarDbContext(userId);

        // Produto
        repositorioProduto = new RepositorioProdutoEmOrm(
            dbContext,
            new ProvedorDeUsuarioFake(userId)
        );

        BuilderSetup.SetCreatePersistenceMethod<Produto>(
            repositorioProduto.Cadastrar);

        BuilderSetup.SetCreatePersistenceMethod<IList<Produto>>((produtos) =>
        {
            foreach (Produto produto in produtos)
                repositorioProduto.Cadastrar(produto);
        });

        // Mesa (descomentar quando o RepositorioMesaEmOrm do Pedro for mesclado)
        repositorioMesa = new RepositorioMesaEmOrm(dbContext, new ProvedorDeUsuarioFake(userId));
        BuilderSetup.SetCreatePersistenceMethod<Mesa>(repositorioMesa.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Mesa>>((mesas) =>
        {
            foreach (Mesa mesa in mesas)
                repositorioMesa.Cadastrar(mesa);
        });

        // Garcom (descomentar quando o RepositorioGarcomEmOrm do Pedro for mesclado)
        repositorioGarcom = new RepositorioGarcomEmOrm(dbContext, new ProvedorDeUsuarioFake(userId));
        BuilderSetup.SetCreatePersistenceMethod<Garcom>(repositorioGarcom.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Garcom>>((garcons) =>
        {
            foreach (Garcom garcom in garcons)
                repositorioGarcom.Cadastrar(garcom);
        });

        // Conta (descomentar quando o RepositorioContaEmOrm do Pedro for mesclado)
        repositorioConta = new RepositorioContaEmOrm(dbContext, new ProvedorDeUsuarioFake(userId));
        BuilderSetup.SetCreatePersistenceMethod<Conta>(repositorioConta.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Conta>>((contas) =>
        {
            foreach (Conta conta in contas)
                repositorioConta.Cadastrar(conta);
        });

        // PedidoConta (descomentar quando disponível)
        repositorioPedidoConta = new RepositorioPedidoContaEmOrm(dbContext, new ProvedorDeUsuarioFake(userId));
        BuilderSetup.SetCreatePersistenceMethod<PedidoConta>(repositorioPedidoConta.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<PedidoConta>>((pedidos) =>
        {
            foreach (PedidoConta pedido in pedidos)
                repositorioPedidoConta.Cadastrar(pedido);
        });
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
