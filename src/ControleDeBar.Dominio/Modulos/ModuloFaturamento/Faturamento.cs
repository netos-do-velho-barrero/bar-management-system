using ControleDeBar.Dominio.Modulos.ModuloConta;

public class Faturamento
{
    public DateTime Data { get; set; }
    public decimal ValorTotal { get; set; }
    public int QuantidadeContasFechadas { get; set; }

    public Faturamento() { }

    public Faturamento(DateTime data, decimal valorTotal, int quantidadeContasFechadas)
    {
        Data = data;
        ValorTotal = valorTotal;
        QuantidadeContasFechadas = quantidadeContasFechadas;
    }

    public static Faturamento Calcular(DateTime data, List<Conta> contasFechadasDoDia)
    {
        return new Faturamento(
            data,
            contasFechadasDoDia.Sum(c => c.ValorTotal),
            contasFechadasDoDia.Count
        );
    }
}
