using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloMesa;

public sealed class RepositorioMesaEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Mesa>(dbContext, provedorDeUsuario), IRepositorioMesa
{
    public void AlterarStatus(Guid mesaId, StatusMesa novoStatus)
    {
        Mesa? mesa = SelecionarPorId(mesaId);

        if (mesa == null)
            return;

        mesa.Status = novoStatus;

        dbContext.SaveChanges();
    }
}
