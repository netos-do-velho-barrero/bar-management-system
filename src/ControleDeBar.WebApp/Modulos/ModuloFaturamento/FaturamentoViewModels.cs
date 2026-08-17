using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloFaturamento;
public class FaturamentoDiarioViewModel
{
    [Display(Name = "Data do Faturamento")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; } = DateTime.Today;

    public decimal TotalFaturado { get; set; }
    public int QuantidadeContas { get; set; }
    public decimal TicketMedio => QuantidadeContas > 0 ? TotalFaturado / QuantidadeContas : 0;

    public List<ItemFaturamentoContaViewModel> ContasFechadas { get; set; } = new();
}

public class ItemFaturamentoContaViewModel
{
    public Guid Id { get; set; }
    public int NumeroMesa { get; set; }
    public string NomeGarcom { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public decimal ValorTotal { get; set; }
}
