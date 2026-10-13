using System.Security.Claims;
using Cad.Contratos.Cuentas;
using Cad.Server.Persistencia.Identidad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cad.Server.Compartido.Seguridad;

public static class ConfiguracionSeguridad
{
    /// <summary>JWT Bearer para la app móvil y cookie para el panel web de central (sección 3.8).</summary>
    public static IServiceCollection AddSeguridadCad(this IServiceCollection services, IConfiguration configuracion)
    {
        services.AddOptions<OpcionesJwt>()
            .Bind(configuracion.GetSection(OpcionesJwt.Seccion))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<EmisorJwt>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer()
            .AddCookie(Politicas.EsquemaPanel, o =>
            {
                o.Cookie.Name = "cad.panel";
                o.Cookie.HttpOnly = true;
                o.Cookie.SameSite = SameSiteMode.Strict;
                o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                o.LoginPath = "/cuentas/login";
                o.LogoutPath = "/cuentas/salir";
                o.AccessDeniedPath = "/cuentas/acceso-denegado";
                o.ExpireTimeSpan = TimeSpan.FromHours(10);
                o.SlidingExpiration = true;
                o.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = ValidarCookieAsync,
                    // La API y el Hub responden 401/403; solo las páginas redirigen al login.
                    OnRedirectToLogin = c => RedirigirOResponder(c, StatusCodes.Status401Unauthorized),
                    OnRedirectToAccessDenied = c => RedirigirOResponder(c, StatusCodes.Status403Forbidden)
                };
            });

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
                o.Events = new JwtBearerEvents { OnTokenValidated = ValidarTokenAsync };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(Politicas.PanelCentral, p => p
                .AddAuthenticationSchemes(Politicas.EsquemaPanel)
                .RequireRole(Roles.Operador, Roles.Administrador))
            .AddPolicy(Politicas.OperadorCentral, p => p
                .AddAuthenticationSchemes(Politicas.EsquemaPanel)
                .RequireRole(Roles.Operador))
            .AddPolicy(Politicas.ConexionTiempoReal, p => p
                .AddAuthenticationSchemes(Politicas.EsquemaPanel, JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        return services;
    }

    /// <summary>Identidad del panel con los mismos claims que el JWT (sub, role y sello de seguridad).</summary>
    public static ClaimsPrincipal CrearPrincipalPanel(Usuario usuario, string rol) =>
        new(new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimNombre, usuario.Email ?? ""),
                new Claim(EmisorJwt.ClaimRol, rol),
                new Claim(EmisorJwt.ClaimSello, usuario.SecurityStamp ?? "")
            ],
            Politicas.EsquemaPanel, ClaimNombre, EmisorJwt.ClaimRol));

    private const string ClaimNombre = "name";

    private static async Task ValidarTokenAsync(TokenValidatedContext contexto)
    {
        if (!await CuentaVigenteAsync(contexto.Principal, contexto.HttpContext.RequestServices))
            contexto.Fail("La sesión ya no es válida.");
    }

    private static async Task ValidarCookieAsync(CookieValidatePrincipalContext contexto)
    {
        if (await CuentaVigenteAsync(contexto.Principal, contexto.HttpContext.RequestServices))
            return;

        contexto.RejectPrincipal();
        await contexto.HttpContext.SignOutAsync(Politicas.EsquemaPanel);
    }

    // Cada solicitud comprueba que la cuenta siga activa y que el sello no haya cambiado (sección 9.6).
    private static async Task<bool> CuentaVigenteAsync(ClaimsPrincipal? principal, IServiceProvider servicios)
    {
        var id = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var sello = principal?.FindFirstValue(EmisorJwt.ClaimSello);
        var usuarios = servicios.GetRequiredService<UserManager<Usuario>>();
        var usuario = id is null ? null : await usuarios.FindByIdAsync(id);

        return usuario is not null && usuario.Activo && usuario.SecurityStamp == sello;
    }

    private static Task RedirigirOResponder(RedirectContext<CookieAuthenticationOptions> contexto, int estado)
    {
        var ruta = contexto.Request.Path;
        if (ruta.StartsWithSegments("/api") || ruta.StartsWithSegments("/hubs"))
            contexto.Response.StatusCode = estado;
        else
            contexto.Response.Redirect(contexto.RedirectUri);

        return Task.CompletedTask;
    }
}
