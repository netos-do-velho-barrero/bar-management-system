using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloMesa;

public interface IRepositorioMesa : IRepositorio<Mesa>
{
    void AlterarStatus(Guid mesaId, StatusMesa novoStatus);
}
