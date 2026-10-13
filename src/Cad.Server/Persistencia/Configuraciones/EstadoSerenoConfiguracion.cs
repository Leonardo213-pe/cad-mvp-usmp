using Cad.Server.Modulos.Despacho.Dominio;
using Cad.Server.Persistencia.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class EstadoSerenoConfiguracion : IEntityTypeConfiguration<EstadoSereno>
{
    public void Configure(EntityTypeBuilder<EstadoSereno> builder)
    {
        builder.ToTable("estados_sereno", t =>
        {
            t.HasCheckConstraint("ck_estados_sereno_latitud", "latitud IS NULL OR latitud BETWEEN -90 AND 90");
            t.HasCheckConstraint("ck_estados_sereno_longitud", "longitud IS NULL OR longitud BETWEEN -180 AND 180");
            // Coordenadas, precisión y hora de obtención: todas nulas o todas presentes (tabla 23).
            t.HasCheckConstraint("ck_estados_sereno_posicion_completa",
                "(latitud IS NULL) = (longitud IS NULL) AND (latitud IS NULL) = (precision_metros IS NULL) " +
                "AND (latitud IS NULL) = (posicion_obtenida_en IS NULL)");
        });

        builder.HasKey(e => e.UsuarioId);

        builder.HasOne<Usuario>()
            .WithOne()
            .HasForeignKey<EstadoSereno>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.ActualizadoEn).IsRequired();
    }
}
