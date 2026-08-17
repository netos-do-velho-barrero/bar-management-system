using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloPedidoConta;

public sealed class PedidoContaConfiguration : IEntityTypeConfiguration<PedidoConta>
{
    public void Configure(EntityTypeBuilder<PedidoConta> builder)
    {
        builder.ToTable("TBPedidoConta");

        builder.HasKey(p => p.Id)
            .HasName("PK_TBPedidoConta");

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.Quantidade)
            .IsRequired();

        
        builder.Property(p => p.PrecoUnitario)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");



        builder.HasOne(p => p.Produto)
            .WithMany(pr => pr.Pedidos)
            .HasForeignKey("ProdutoId")
            .HasConstraintName("FK_TBPedidoConta_TBProduto")
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasOne(p => p.Conta)
            .WithMany(c => c.Pedidos)
            .HasForeignKey("ContaId")
            .HasConstraintName("FK_TBPedidoConta_TBConta")
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
