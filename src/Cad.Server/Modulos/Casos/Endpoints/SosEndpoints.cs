using System.Security.Claims;
using Cad.Contratos.Cuentas;
using Cad.Contratos.Sos;
using Cad.Server.Compartido;
using Cad.Server.Modulos.Casos.Aplicacion;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Cad.Server.Modulos.Casos.Endpoints;

public static class SosEndpoints
{
    public static IEndpointRouteBuilder MapSosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/sos", async (EmitirSosSolicitud solicitud, ClaimsPrincipal usuario, ServicioSos servicio, CancellationToken ct) =>
            {
                if (!Guid.TryParse(usuario.FindFirstValue(JwtRegisteredClaimNames.Sub), out var ciudadanoId))
                    return Results.Unauthorized();

                var resultado = await servicio.EmitirAsync(ciudadanoId, solicitud, ct);
                if (!resultado.EsExito)
                    return resultado.Error!.ARespuesta();

                var (caso, creado) = resultado.Valor!;
                var respuesta = new SosEmitidoRespuesta(caso.Id, caso.Estado.ToString(), caso.CreadoEn, caso.TieneUbicacion);

                // 201 la primera vez; 200 con el mismo SOS cuando es un reintento (RNF08).
                return Results.Json(respuesta, statusCode: creado ? StatusCodes.Status201Created : StatusCodes.Status200OK);
            })
            .RequireAuthorization(p => p.RequireRole(Roles.Ciudadano))
            .WithName("EmitirSos");

        return app;
    }
}
