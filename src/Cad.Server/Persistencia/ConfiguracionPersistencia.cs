using Cad.Server.Persistencia.Identidad;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cad.Server.Persistencia;

public static class ConfiguracionPersistencia
{
    public static IServiceCollection AddPersistenciaCad(this IServiceCollection services, IConfiguration configuracion)
    {
        var cadena = configuracion.GetConnectionString("Cad")
            ?? throw new InvalidOperationException("Falta la cadena de conexión ConnectionStrings__Cad.");

        services.AddDbContext<CadDbContext>(o => o.UseNpgsql(cadena).UseSnakeCaseNamingConvention());

        services.AddIdentityCore<Usuario>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 8;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<CadDbContext>()
            .AddErrorDescriber<DescriptorErroresIdentidad>();

        // RNF01: formato V3 de Identity = PBKDF2-HMAC-SHA512, sal de 128 bits, subclave de 256 bits.
        services.Configure<PasswordHasherOptions>(o =>
        {
            o.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
            o.IterationCount = 100_000;
        });

        return services;
    }

    /// <summary>Aplica las migraciones pendientes al arrancar (una sola instancia, sección 4).</summary>
    public static async Task MigrarBaseDeDatosAsync(this WebApplication app)
    {
        await using var alcance = app.Services.CreateAsyncScope();
        var db = alcance.ServiceProvider.GetRequiredService<CadDbContext>();
        await db.Database.MigrateAsync();
    }
}
