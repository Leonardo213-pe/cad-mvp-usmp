namespace Cad.Server.Compartido.Seguridad;

/// <summary>Esquemas y políticas de autorización con nombre propio (HU03, DP04).</summary>
public static class Politicas
{
    /// <summary>Cookie HttpOnly del panel web de central (sección 9.6). La app usa JWT Bearer.</summary>
    public const string EsquemaPanel = "Panel";

    /// <summary>Páginas del panel: operador o administrador con sesión de panel.</summary>
    public const string PanelCentral = "PanelCentral";

    /// <summary>Sección urgente y consultas de la central: solo operador (con sesión de panel).</summary>
    public const string OperadorCentral = "OperadorCentral";

    /// <summary>Conexión al Hub: cualquier cuenta autenticada, por cookie (panel) o JWT (app).</summary>
    public const string ConexionTiempoReal = "ConexionTiempoReal";
}
