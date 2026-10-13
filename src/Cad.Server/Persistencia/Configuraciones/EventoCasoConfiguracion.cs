using Cad.Server.Modulos.Casos.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class EventoCasoConfiguracion : IEntityTypeConfiguration<EventoCaso>
{
    public void Configure(EntityTypeBuilder<EventoCaso> builder)
    {
        builder.ToTable("eventos_caso");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Tipo).HasMaxLength(EventoCaso.LongitudMaximaTipo).IsRequired();
        builder.Property(e => e.Visibilidad).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Datos).HasColumnType("jsonb");
        builder.Property(e => e.OcurridoEn).IsRequired();

        builder.HasOne<Caso>()
            .WithMany()
            .HasForeignKey(e => e.CasoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.CasoId, e.OcurridoEn });
    }
}
