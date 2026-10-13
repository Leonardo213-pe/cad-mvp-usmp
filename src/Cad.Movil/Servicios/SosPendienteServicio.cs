using System.Text.Json;
using Cad.Contratos.Sos;

namespace Cad.Movil.Servicios;

/// <summary>
/// Guarda el SOS que todavía no confirmó el servidor, con su IdSolicitud, para reintentarlo sin duplicarlo
/// (RNF08), incluso si la app se cierra. Va en SecureStorage porque puede llevar coordenadas (RNF07).
/// </summary>
public sealed class SosPendienteServicio(ISecureStorage almacen)
{
    private const string Clave = "sos_pendiente";

    /// <summary>Devuelve el SOS pendiente solo si es del usuario indicado; el de otra cuenta se descarta.</summary>
    public async Task<EmitirSosSolicitud?> ObtenerAsync(Guid usuarioId)
    {
        try
        {
            var json = await almacen.GetAsync(Clave);
            var guardado = json is null ? null : JsonSerializer.Deserialize<SosGuardado>(json);
            if (guardado is null)
                return null;

            if (guardado.UsuarioId != usuarioId)
            {
                Borrar();
                return null;
            }

            return guardado.Solicitud;
        }
        catch (Exception)
        {
            Borrar();
            return null;
        }
    }

    public Task GuardarAsync(Guid usuarioId, EmitirSosSolicitud solicitud) =>
        almacen.SetAsync(Clave, JsonSerializer.Serialize(new SosGuardado(usuarioId, solicitud)));

    public void Borrar() => almacen.Remove(Clave);

    private sealed record SosGuardado(Guid UsuarioId, EmitirSosSolicitud Solicitud);
}
