using Cad.Server.Modulos.Casos.Dominio;
using Cad.Server.Persistencia.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class CasoConfiguracion : IEntityTypeConfiguration<Caso>
{
    public void Configure(EntityTypeBuilder<Caso> builder)
    {
        builder.ToTable("casos", t =>
        {
            t.HasCheckConstraint("ck_casos_latitud", "latitud IS NULL OR latitud BETWEEN -90 AND 90");
            t.HasCheckConstraint("ck_casos_longitud", "longitud IS NULL OR longitud BETWEEN -180 AND 180");
            t.HasCheckConstraint("ck_casos_precision", "precision_metros IS NULL OR precision_metros >= 0");
            // Tabla 23: coordenadas, precisión y hora de obtención son todas nulas o todas presentes.
            t.HasCheckConstraint("ck_casos_ubicacion_completa",
                "(latitud IS NULL) = (longitud IS NULL) AND (latitud IS NULL) = (precision_metros IS NULL) " +
                "AND (latitud IS NULL) = (ubicacion_obtenida_en IS NULL)");
        });

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Origen).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.CreadoEn).IsRequired();

        // xmin de PostgreSQL como token de concurrencia optimista (ADR-03).
        builder.Property(c => c.Version).IsRowVersion();

        builder.Ignore(c => c.TieneUbicacion);

        // RNF08: un reintento con el mismo IdSolicitud no crea otro caso.
        builder.HasIndex(c => new { c.CiudadanoId, c.IdSolicitud })
            .IsUnique()
            .HasDatabaseName("ux_casos_ciudadano_id_solicitud");

        builder.HasIndex(c => c.Estado);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.CiudadanoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
