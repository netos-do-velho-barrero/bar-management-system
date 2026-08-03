using GeradorDeProvas.Infra.Compartilhado.Orm;
using GeradorDeProvas.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GeradorDeProvas.Testes.E2E.Compartilhado;

public sealed class TestApplicationFactory : WebApplicationFactory<Entrypoint>
{
    private readonly string nomeBanco;
    private readonly InMemoryDatabaseRoot dbRoot;

    public string UrlBase { get; }

    public TestApplicationFactory()
    {
        nomeBanco = $"e2e-{Guid.NewGuid():N}";
        dbRoot = new InMemoryDatabaseRoot();

        UseKestrel(0);
        StartServer();

        UrlBase = ObterUrlKestrel();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Infra:NewRelic:Enabled", "false");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<GeradorDeProvasDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<GeradorDeProvasDbContext>>();

            services.AddDbContext<GeradorDeProvasDbContext>(options =>
            {
                options.UseInMemoryDatabase(nomeBanco, dbRoot);
            });
        });
    }

    private string ObterUrlKestrel()
    {
        IServer servidor = Services.GetRequiredService<IServer>();

        IServerAddressesFeature? enderecos = servidor.Features.Get<IServerAddressesFeature>();

        if (enderecos == null)
            throw new InvalidOperationException("Não foi possível obter a URL do servidor");

        return enderecos.Addresses.Single();
    }
}
