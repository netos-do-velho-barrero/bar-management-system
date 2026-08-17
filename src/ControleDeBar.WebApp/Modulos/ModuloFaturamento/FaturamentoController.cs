using Microsoft.AspNetCore.Mvc;
using AutoMapper;

// Namespace da camada de Aplicação (onde estão os DTOs)
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

namespace ControleDeBar.WebApp.Modulos.ModuloFaturamento;

public class FaturamentoController : Controller
{
    private readonly ServicoFaturamento servicoFaturamento;
    private readonly IMapper mapper;

    public FaturamentoController(ServicoFaturamento servicoFaturamento, IMapper mapper)
    {
        this.servicoFaturamento = servicoFaturamento;
        this.mapper = mapper;
    }

    [HttpGet]
    public IActionResult Diario(DateTime? data)
    {
        var dataFiltro = data ?? DateTime.Today;

        // FaturamentoDiarioDto vem do namespace da Aplicação
        var dto = servicoFaturamento.ObterFaturamentoDiario(dataFiltro);

        // Se o FaturamentoDiarioViewModel estiver no mesmo namespace do Controller,
        // ele é reconhecido automaticamente sem precisar de using extra.
        var viewModel = mapper.Map<FaturamentoDiarioViewModel>(dto);

        return View(viewModel);
    }
}
