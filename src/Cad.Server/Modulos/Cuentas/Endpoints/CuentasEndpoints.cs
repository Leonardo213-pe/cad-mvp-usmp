using Cad.Contratos.Cuentas;
using Cad.Server.Compartido;
using Cad.Server.Modulos.Cuentas.Aplicacion;

namespace Cad.Server.Modulos.Cuentas.Endpoints;

public static class CuentasEndpoints
{
    public static IEndpointRouteBuilder MapCuentasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cuentas/registro", async (RegistroCiudadanoSolicitud solicitud, ServicioCuentas servicio, CancellationToken ct) =>
            {
                var resultado = await servicio.RegistrarCiudadanoAsync(solicitud, ct);
                return resultado.EsExito
                    ? Results.Created((string?)null, resultado.Valor)
                    : resultado.Error!.ARespuesta();
            })
            .AllowAnonymous()
            .WithName("RegistrarCiudadano");

        app.MapPost("/api/auth/token", async (TokenSolicitud solicitud, ServicioCuentas servicio, CancellationToken ct) =>
            {
                var resultado = await servicio.EmitirTokenAsync(solicitud, ct);
                return resultado.EsExito ? Results.Ok(resultado.Valor) : resultado.Error!.ARespuesta();
            })
            .AllowAnonymous()
            .WithName("EmitirToken");

        return app;
    }
}
