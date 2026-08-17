using Microsoft.AspNetCore.Mvc;
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

namespace ControleDeBar.WebApp.Modulos.ModuloFaturamento;

public class FaturamentoController(ServicoFaturamento servicoFaturamento) : Controller
{
    public IActionResult Diario(DateTime? data)
    {
        var dataFiltro = data ?? DateTime.Today;
        var dto = servicoFaturamento.ObterFaturamentoDiario(dataFiltro);

        var viewModel = new FaturamentoDiarioViewModel
        {
            Data = dto.Data,
            TotalFaturado = dto.ValorTotal,
            QuantidadeContas = dto.QuantidadeContas,
            ContasFechadas = dto.ContasFechadas.Select(item => new ItemFaturamentoContaViewModel
            {
                Id = item.Id,
                NumeroMesa = item.NumeroMesa,
                NomeGarcom = item.NomeGarcom,
                NomeCliente = item.NomeCliente,
                DataAbertura = item.DataAbertura,
                DataFechamento = item.DataFechamento,
                ValorTotal = item.ValorTotal
            }).ToList()
        };

        return View(viewModel);
    }
}
