using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleDeBar.Aplicacao;

public static class InjecaoDeDependencia
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ServicoMesa>();
        services.AddScoped<ServicoGarcom>();
        services.AddScoped<ServicoConta>();
        services.AddScoped<ServicoProduto>();
        services.AddScoped<ServicoPedidoConta>();
        services.AddScoped<ServicoFaturamento>();
    }
}
