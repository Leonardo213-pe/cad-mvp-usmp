using Cad.Server.Modulos.Casos.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class MedicionPresentacionConfiguracion : IEntityTypeConfiguration<MedicionPresentacion>
{
    public void Configure(EntityTypeBuilder<MedicionPresentacion> builder)
    {
        builder.ToTable("mediciones_presentacion");
        builder.HasKey(m => m.CasoId);

        builder.HasOne<Caso>()
            .WithOne()
            .HasForeignKey<MedicionPresentacion>(m => m.CasoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
