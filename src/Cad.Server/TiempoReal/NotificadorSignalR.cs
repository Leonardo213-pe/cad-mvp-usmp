using Cad.Contratos.TiempoReal;
using Cad.Server.Compartido;
using Cad.Server.Modulos.Consultas.Aplicacion;
using Microsoft.AspNetCore.SignalR;

namespace Cad.Server.TiempoReal;

/// <summary>Implementación de <see cref="INotificador"/> con SignalR (sección 6, inversión de dependencia).</summary>
public sealed class NotificadorSignalR(IHubContext<CadHub> hub, ConsultasCentral consultas) : INotificador
{
    public async Task SosNuevoAsync(Guid casoId, CancellationToken ct)
    {
        var sos = await consultas.SosAsync(casoId, ct);
        if (sos is null)
            return;

        await hub.Clients.Group(GruposTiempoReal.Operadores).SendAsync(TiempoRealCad.EventoSosNuevo, sos, ct);
    }
}
