using Cad.Server.Modulos.Cuentas.Dominio;
using Cad.Server.Persistencia.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class PerfilCiudadanoConfiguracion : IEntityTypeConfiguration<PerfilCiudadano>
{
    public void Configure(EntityTypeBuilder<PerfilCiudadano> builder)
    {
        builder.ToTable("perfiles_ciudadano");
        builder.HasKey(p => p.UsuarioId);

        builder.HasOne<Usuario>()
            .WithOne()
            .HasForeignKey<PerfilCiudadano>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.NombreCompleto).HasMaxLength(PerfilCiudadano.LongitudMaximaNombre).IsRequired();
        builder.Property(p => p.Telefono).HasMaxLength(PerfilCiudadano.LongitudMaximaTelefono);
        builder.Property(p => p.RegistradoEn).IsRequired();
    }
}
