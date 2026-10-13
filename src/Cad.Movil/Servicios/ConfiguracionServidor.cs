namespace Cad.Movil.Servicios;

/// <summary>URL del Servidor CAD, configurable desde la pantalla de inicio de sesión.</summary>
public sealed class ConfiguracionServidor(IPreferences preferencias)
{
    private const string Clave = "servidor_url";

#if DEBUG
    // Con el teléfono por USB: `adb reverse tcp:5080 tcp:5080` hace que localhost apunte al PC.
    public const string UrlPorDefecto = "http://localhost:5080";
#else
    public const string UrlPorDefecto = "";
#endif

    public string Url
    {
        get => preferencias.Get(Clave, UrlPorDefecto);
        set => preferencias.Set(Clave, value.Trim());
    }
}
