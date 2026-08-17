using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloPedidoConta;

public sealed class RepositorioPedidoContaEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<PedidoConta>(dbContext, provedorDeUsuario), IRepositorioPedidoConta;
