using System.Reflection;
using ControleDeBar.Dominio.Compartilhado.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Compartilhado.Orm;

public sealed class ControleDeBarDbContext(
    DbContextOptions<ControleDeBarDbContext> options,
    IProvedorDeUsuario? userProvider = null
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Assembly assembly = typeof(ControleDeBarDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
}
