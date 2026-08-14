using ControleDeBar.Dominio.Modulos.ModuloFaturamento;

namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

// Não herda ServicoBase<T> porque Faturamento não é uma EntidadeBase — é uma
// consulta agregada somente-leitura, sem Cadastrar/Editar/Excluir/Validar.
public class ServicoFaturamento(
    IRepositorioFaturamento repositorioFaturamento
)
{
    public FaturamentoDiarioDto ObterFaturamentoDiario(DateTime data)
    {
        Faturamento faturamento = repositorioFaturamento.ObterFaturamentoDiario(data);

        return new FaturamentoDiarioDto(
            faturamento.Data,
            faturamento.ValorTotal,
            faturamento.QuantidadeContasFechadas
        );
    }
}
