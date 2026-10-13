using Cad.Contratos.Cuentas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cad.Server.Persistencia.Configuraciones;

public sealed class RolConfiguracion : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("roles");

        // Identificadores fijos para que la semilla sea estable entre migraciones.
        builder.HasData(
            Rol("6d1e0b7a-4c1f-4f3e-9a51-000000000001", Roles.Ciudadano),
            Rol("6d1e0b7a-4c1f-4f3e-9a51-000000000002", Roles.Operador),
            Rol("6d1e0b7a-4c1f-4f3e-9a51-000000000003", Roles.Sereno),
            Rol("6d1e0b7a-4c1f-4f3e-9a51-000000000004", Roles.Administrador));
    }

    private static IdentityRole<Guid> Rol(string id, string nombre) => new()
    {
        Id = Guid.Parse(id),
        Name = nombre,
        NormalizedName = nombre.ToUpperInvariant(),
        ConcurrencyStamp = id
    };
}
