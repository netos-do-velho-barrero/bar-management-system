using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp;

[AllowAnonymous] // <--- Permite acessar a Home sem precisar estar logado
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
