using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Logging;
using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Infra.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Infra.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Infra.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloFaturamento;
using ControleDeBar.Infra.Modulos.ModuloFaturamento;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ControleDeBar.Infra;

public static class InjecaoDeDependencia
{
    public static void AddInfraRepositories(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder logging,
        IHostEnvironment environment
    )
    {
        // Injeta logs do Serilog
        Serilog.ILogger logger = SerilogFactory.Create(configuration, environment);

        logging.ClearProviders();

        services.AddSerilog(logger, dispose: true);

        // Injeta o DbContext do EF
        services.AddDbContext<ControleDeBarDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("SqlServerEF");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"A connection string \"SqlServerEF\" não foi encontrada."
                );
            }

            options.UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure(3);
            });
        });

        // Configuração do Usuário no Identity
        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ControleDeBarDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // Repositórios
        services.AddScoped<IRepositorioMesa, RepositorioMesaEmOrm>();
        services.AddScoped<IRepositorioGarcom, RepositorioGarcomEmOrm>();
        services.AddScoped<IRepositorioConta, RepositorioContaEmOrm>();
        services.AddScoped<IRepositorioProduto, RepositorioProdutoEmOrm>();
        services.AddScoped<IRepositorioPedidoConta, RepositorioPedidoContaEmOrm>();
        services.AddScoped<IRepositorioFaturamento, RepositorioFaturamentoEmOrm>();
    }
}

