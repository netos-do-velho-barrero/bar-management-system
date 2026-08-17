using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using ControleDeBar.WebApp.Modulos.ModuloConta;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloPedidoConta;

// Não tem Listar/Cadastrar/Editar como tela própria — os pedidos aparecem e são
// gerenciados dentro de Conta/Detalhes. Todas as ações aqui redirecionam de
// volta para lá (ContaController.Detalhes) usando o ContaId recebido no ViewModel.
public class PedidoContaController(
    ServicoPedidoConta servicoPedidoConta,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Adicionar(Guid contaId)
    {
        AdicionarPedidoContaViewModel vm = new() { ContaId = contaId };

        CarregarProdutos();

        return View(vm);
    }

    [HttpPost]
    public ActionResult Adicionar(AdicionarPedidoContaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            CarregarProdutos();

            return View(vm);
        }

        AdicionarPedidoContaDto dto =
            mapeador.Map<AdicionarPedidoContaDto>(vm);

        Result resultado =
            servicoPedidoConta.Adicionar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            CarregarProdutos();

            return View(vm);
        }

        return RedirectToAction(
            nameof(ContaController.Detalhes),
            "Conta",
            new { id = vm.ContaId }
        );
    }

    [HttpPost]
    public ActionResult EditarQuantidade(EditarQuantidadePedidoContaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData.AddErrorMessage(
                Result.Fail("Quantidade inválida.")
            );

            return RedirectToAction(
                nameof(ContaController.Detalhes),
                "Conta",
                new { id = vm.ContaId }
            );
        }

        EditarQuantidadePedidoContaDto dto =
            mapeador.Map<EditarQuantidadePedidoContaDto>(vm);

        Result resultado =
            servicoPedidoConta.EditarQuantidade(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(
            nameof(ContaController.Detalhes),
            "Conta",
            new { id = vm.ContaId }
        );
    }

    [HttpPost]
    public ActionResult Remover(Guid id, Guid contaId)
    {
        Result resultado =
            servicoPedidoConta.Remover(id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(
            nameof(ContaController.Detalhes),
            "Conta",
            new { id = contaId }
        );
    }

    private void CarregarProdutos()
    {
        ViewBag.Produtos =
            mapeador.Map<List<OpcaoProdutoPedidoViewModel>>(
                servicoPedidoConta.SelecionarProdutosDisponiveis()
            );
    }
}
