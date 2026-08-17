using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.WebApp.Compartilhado.Extensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public class ContaController(
    ServicoConta servicoConta,
    IMapper mapeador
) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarContaDto> dtos =
            servicoConta.SelecionarTodos();

        List<ListarContaViewModel> vms =
            mapeador.Map<List<ListarContaViewModel>>(dtos);

        return View(vms);
    }

    [HttpGet]
    public ActionResult Abrir()
    {
        AbrirContaViewModel vm = new();

        CarregarOpcoes();

        return View(vm);
    }

    [HttpPost]
    public ActionResult Abrir(AbrirContaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            CarregarOpcoes();

            return View(vm);
        }

        AbrirContaDto dto =
            mapeador.Map<AbrirContaDto>(vm);

        Result resultado =
            servicoConta.Abrir(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            CarregarOpcoes();

            return View(vm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesContaDto> resultado =
            servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarContaViewModel vm =
            mapeador.Map<EditarContaViewModel>(
                resultado.Value
            );

        CarregarOpcoes(id);

        return View(vm);
    }

    [HttpPost]
    public ActionResult Editar(EditarContaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            CarregarOpcoes(vm.Id);

            return View(vm);
        }

        EditarContaDto dto =
            mapeador.Map<EditarContaDto>(vm);

        Result resultado =
            servicoConta.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            CarregarOpcoes(vm.Id);

            return View(vm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Detalhes(Guid id)
    {
        Result<DetalhesContaDto> resultado =
            servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        DetalhesContaViewModel vm =
            mapeador.Map<DetalhesContaViewModel>(
                resultado.Value
            );

        return View(vm);
    }

    [HttpGet]
    public ActionResult ConfirmarFechamento(Guid id)
    {
        Result<DetalhesContaDto> resultado =
            servicoConta.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        DetalhesContaViewModel vm =
            mapeador.Map<DetalhesContaViewModel>(
                resultado.Value
            );

        return View("Fechar", vm);
    }

    [HttpPost]
    public ActionResult Fechar(Guid id)
    {
        Result resultado =
            servicoConta.Fechar(id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }

    private void CarregarOpcoes(Guid? contaId = null)
    {
        ViewBag.Mesas =
            mapeador.Map<List<OpcaoMesaContaViewModel>>(
                servicoConta.SelecionarMesasDisponiveis(contaId)
            );

        ViewBag.Garcons =
            mapeador.Map<List<OpcaoGarcomContaViewModel>>(
                servicoConta.SelecionarGarcons()
            );
    }
}
