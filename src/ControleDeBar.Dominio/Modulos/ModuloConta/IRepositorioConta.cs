using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public interface IRepositorioConta : IRepositorio<Conta>
{
    void AlterarSituacao(Guid contaId, SituacaoConta novaSituacao);
    List<Conta> SelecionarAbertas();
    List<Conta> SelecionarFechadas();
}
