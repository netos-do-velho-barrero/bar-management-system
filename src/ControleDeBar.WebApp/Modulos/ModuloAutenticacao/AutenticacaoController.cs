using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Modulos.ModuloAutenticacao;

[AllowAnonymous] // ISSO AQUI QUEBRA O LOOP INFINITO
public class AutenticacaoController : Controller
{
    [HttpGet]
    public IActionResult Entrar()
    {
        return View();
    }
}
