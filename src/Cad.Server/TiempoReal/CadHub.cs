using Cad.Contratos.Cuentas;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Casos.Aplicacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Cad.Server.TiempoReal;

public static class GruposTiempoReal
{
    public const string Operadores = "operadores";
}

/// <summary>
/// Hub /hubs/cad (ADR-04): solo envía eventos. Las escrituras van por REST.
/// El cliente no elige sus grupos: el servidor lo une según el rol de su sesión (sección 9.5).
/// </summary>
[Authorize(Policy = Politicas.ConexionTiempoReal)]
public sealed class CadHub(ServicioPresentacion presentacion) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User?.IsInRole(Roles.Operador) == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, GruposTiempoReal.Operadores);

        await base.OnConnectedAsync();
    }

    /// <summary>Acuse del panel: el SOS ya está en la sección urgente. Sirve para medir RNF02.</summary>
    public async Task Presentado(Guid casoId)
    {
        if (Context.User?.IsInRole(Roles.Operador) != true
            || !Guid.TryParse(Context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var operadorId))
            throw new HubException("Solo un operador puede registrar la presentación de un SOS.");

        await presentacion.RegistrarAsync(casoId, operadorId, Context.ConnectionAborted);
    }
}
