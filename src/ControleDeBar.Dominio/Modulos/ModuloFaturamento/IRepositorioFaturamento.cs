namespace ControleDeBar.Dominio.Modulos.ModuloFaturamento;

// Não estende IRepositorio<T> porque Faturamento não tem Cadastrar/Editar/Excluir —
// é uma consulta agregada somente-leitura sobre as Contas fechadas do bar do
// usuário autenticado (o filtro por UserId/bar é responsabilidade da implementação,
// igual ao que já acontece com os demais repositórios que lidam com IEntidadeDoUsuario).
public interface IRepositorioFaturamento
{
    // Retorna o faturamento consolidado de uma data específica, considerando
    // somente as Contas com Situacao == SituacaoConta.Fechada pertencentes
    // ao bar do usuário autenticado.
    Faturamento ObterFaturamentoDiario(DateTime data);
}






























































