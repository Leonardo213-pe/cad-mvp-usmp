using Cad.Server.Modulos.Cuentas.Dominio;
using Cad.Server.Modulos.Despacho.Dominio;
using Cad.Server.Persistencia.Identidad;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cad.Server.Persistencia;

public sealed class CadDbContext(DbContextOptions<CadDbContext> options)
    : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<PerfilCiudadano> PerfilesCiudadano => Set<PerfilCiudadano>();
    public DbSet<EstadoSereno> EstadosSereno => Set<EstadoSereno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("usuario_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("usuario_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("usuario_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("usuario_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("rol_claims");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CadDbContext).Assembly);
    }
}
