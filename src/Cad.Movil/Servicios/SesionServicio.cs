using System.Text.Json;
using Cad.Contratos.Cuentas;

namespace Cad.Movil.Servicios;

public sealed record SesionActual(string Token, DateTimeOffset ExpiraEn, Guid UsuarioId, string Rol, string Email);

/// <summary>Guarda el JWT en SecureStorage (Android Keystore), nunca en preferencias en texto plano (sección 8.2).</summary>
public sealed class SesionServicio(ISecureStorage almacen)
{
    private const string Clave = "sesion";

    public SesionActual? Actual { get; private set; }

    /// <summary>Recupera la sesión guardada si el token sigue vigente.</summary>
    public async Task<SesionActual?> CargarAsync()
    {
        try
        {
            var json = await almacen.GetAsync(Clave);
            var sesion = json is null ? null : JsonSerializer.Deserialize<SesionActual>(json);
            if (sesion is null || sesion.ExpiraEn <= DateTimeOffset.UtcNow.AddMinutes(1))
            {
                Cerrar();
                return null;
            }

            Actual = sesion;
            return sesion;
        }
        catch (Exception)
        {
            // Almacén dañado o clave del Keystore invalidada: se pide iniciar sesión de nuevo.
            Cerrar();
            return null;
        }
    }

    public async Task GuardarAsync(TokenRespuesta token, string email)
    {
        var sesion = new SesionActual(token.AccessToken, token.ExpiraEn, token.UsuarioId, token.Rol, email);
        await almacen.SetAsync(Clave, JsonSerializer.Serialize(sesion));
        Actual = sesion;
    }

    public void Cerrar()
    {
        Actual = null;
        almacen.Remove(Clave);
    }
}
