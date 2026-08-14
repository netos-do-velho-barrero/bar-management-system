using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Orm;

namespace ControleDeBar.Infra.Modulos.ModuloMesa;

public sealed class RepositorioMesaEmOrm(
    ControleDeBarDbContext dbContext
) : RepositorioBaseEmOrm<Mesa>(dbContext), IRepositorioMesa
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
