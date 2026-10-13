using System.Security.Claims;
using Cad.Server.Persistencia.Identidad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cad.Server.Compartido.Seguridad;

public static class ConfiguracionSeguridad
{
    /// <summary>Autenticación JWT Bearer para la app móvil (la cookie del panel llega con su rama).</summary>
    public static IServiceCollection AddSeguridadCad(this IServiceCollection services, IConfiguration configuracion)
    {
        services.AddOptions<OpcionesJwt>()
            .Bind(configuracion.GetSection(OpcionesJwt.Seccion))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<EmisorJwt>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<OpcionesJwt>>((o, jwt) =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Value.Emisor,
                    ValidAudience = jwt.Value.Audiencia,
                    IssuerSigningKey = EmisorJwt.CrearClave(jwt.Value),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = EmisorJwt.ClaimRol,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                o.Events = new JwtBearerEvents { OnTokenValidated = ComprobarCuentaVigenteAsync };
            });

        services.AddAuthorization();
        return services;
    }

    // Cada solicitud comprueba que la cuenta siga activa y que el sello no haya cambiado (sección 9.6).
    private static async Task ComprobarCuentaVigenteAsync(TokenValidatedContext contexto)
    {
        var id = contexto.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var sello = contexto.Principal?.FindFirstValue(EmisorJwt.ClaimSello);
        var usuarios = contexto.HttpContext.RequestServices.GetRequiredService<UserManager<Usuario>>();
        var usuario = id is null ? null : await usuarios.FindByIdAsync(id);

        if (usuario is null || !usuario.Activo || usuario.SecurityStamp != sello)
        {
            contexto.Fail("La sesión ya no es válida.");
        }
    }
}
