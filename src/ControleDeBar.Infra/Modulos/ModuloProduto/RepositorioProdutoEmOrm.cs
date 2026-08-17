using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloProduto;

public sealed class RepositorioProdutoEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Produto>(dbContext, provedorDeUsuario), IRepositorioProduto;
