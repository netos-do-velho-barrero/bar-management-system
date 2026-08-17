using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloGarcom;

public sealed class RepositorioGarcomEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Garcom>(dbContext, provedorDeUsuario), IRepositorioGarcom;
