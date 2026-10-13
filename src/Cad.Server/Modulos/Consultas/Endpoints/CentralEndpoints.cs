using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Consultas.Aplicacion;

namespace Cad.Server.Modulos.Consultas.Endpoints;

public static class CentralEndpoints
{
    public static IEndpointRouteBuilder MapCentralEndpoints(this IEndpointRouteBuilder app)
    {
        // Estado inicial de la sección urgente y resincronización al reconectar (sección 9.8).
        app.MapGet("/api/central/sos-pendientes", async (ConsultasCentral consultas, CancellationToken ct) =>
                Results.Ok(await consultas.SosPendientesAsync(ct)))
            .RequireAuthorization(Politicas.OperadorCentral)
            .WithName("SosPendientes");

        return app;
    }
}
