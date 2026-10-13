using Cad.Server.Persistencia.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        // Identity crea este índice sin unicidad; aquí se exige en la BD para resistir registros concurrentes (HU01).
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("ux_usuarios_email_normalizado").IsUnique();

        builder.Property(u => u.Activo).HasDefaultValue(true);
        builder.Property(u => u.CreadoEn).IsRequired();
    }
}
