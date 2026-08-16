using ControleDeBar.Aplicacao;
using ControleDeBar.Infra;
using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.WebApp.Compartilhado;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração de licença do QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Configuração do container de injeção de dependência
builder.Services.AddInfraRepositories(
    builder.Configuration,
    builder.Logging,
    builder.Environment
);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPresentationConfig(builder.Configuration);

// Configura health checks do banco de dados
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ControleDeBarDbContext>(
        name: "database_check",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"]
    );

var app = builder.Build();

// Aplica migrações automaticamente no ambiente de Desenvolvimento
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ControleDeBarDbContext>();
    dbContext.Database.Migrate();
}

// Arquivos estáticos (CSS, JS, imagens da pasta wwwroot)
app.UseStaticFiles();

// Middlewares de roteamento
app.UseRouting();

// Middlewares de Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// Middleware de reconhecimento de rotas de controllers
app.MapDefaultControllerRoute();

// Execução do Servidor Web
app.Run();

