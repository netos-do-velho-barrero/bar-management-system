namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public record FaturamentoDiarioDto(
    DateTime Data,
    decimal ValorTotal,
    int QuantidadeContasFechadas
);
 