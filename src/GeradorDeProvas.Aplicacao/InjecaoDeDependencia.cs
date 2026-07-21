using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GeradorDeProvas.Aplicacao.Modulos.ModuloDisciplina;

namespace GeradorDeProvas.Aplicacao;

public static class InjecaoDeDependencia
{
    public static void AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ServicoDisciplina>();
    }
}
