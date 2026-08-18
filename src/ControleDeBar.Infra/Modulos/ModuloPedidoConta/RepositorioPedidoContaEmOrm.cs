using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloPedidoConta;

public sealed class RepositorioPedidoContaEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<PedidoConta>(dbContext, provedorDeUsuario), IRepositorioPedidoConta
{
    public override PedidoConta? SelecionarPorId(Guid idSelecionado)
    {
        return RegistrosDoUsuario()
            .Include(p => p.Produto)
            .Include(p => p.Conta)
            .SingleOrDefault(p => p.Id == idSelecionado);
    }

    public override PedidoConta? SelecionarPorIdSemFiltro(Guid idSelecionado)
    {
        return registros
            .Include(p => p.Produto)
            .Include(p => p.Conta)
            .SingleOrDefault(p => p.Id == idSelecionado);
    }

    public override List<PedidoConta> Filtrar(Func<PedidoConta, bool> filtro)
    {
        return RegistrosDoUsuario()
            .Include(p => p.Produto)
            .Include(p => p.Conta)
            .AsEnumerable()
            .Where(filtro)
            .ToList();
    }
}
