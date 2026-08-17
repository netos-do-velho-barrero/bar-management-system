using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public class ProdutoController(
    ServicoProduto servicoProduto,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarProdutoDto> dtos =
            servicoProduto.SelecionarTodos();

        List<ListarProdutoViewModel> listarVms =
            mapeador.Map<List<ListarProdutoViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarProdutoViewModel cadastrarVm = new();

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarProdutoViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        CadastrarProdutoDto dto =
            mapeador.Map<CadastrarProdutoDto>(cadastrarVm);

        Result resultado =
            servicoProduto.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(cadastrarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesProdutoDto> resultado =
            servicoProduto.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarProdutoViewModel editarVm =
            mapeador.Map<EditarProdutoViewModel>(resultado.Value);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarProdutoViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        EditarProdutoDto dto =
            mapeador.Map<EditarProdutoDto>(editarVm);

        Result resultado =
            servicoProduto.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Detalhes(Guid id)
    {
        Result<DetalhesProdutoDto> resultado =
            servicoProduto.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        DetalhesProdutoViewModel detalhesVm =
            mapeador.Map<DetalhesProdutoViewModel>(resultado.Value);

        return View(detalhesVm);
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesProdutoDto> resultado =
            servicoProduto.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        DetalhesProdutoViewModel detalhesVm =
            mapeador.Map<DetalhesProdutoViewModel>(resultado.Value);

        return View(detalhesVm);
    }

    [HttpPost]
    public ActionResult Excluir(DetalhesProdutoViewModel detalhesVm)
    {
        Result resultado =
            servicoProduto.Excluir(detalhesVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }
}
