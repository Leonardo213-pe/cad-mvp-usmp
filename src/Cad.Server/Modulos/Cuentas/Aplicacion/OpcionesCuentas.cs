namespace Cad.Server.Modulos.Cuentas.Aplicacion;

public sealed class OpcionesCuentas
{
    public const string Seccion = "Cuentas";

    /// <summary>DP01 sin resolver: la obligatoriedad del teléfono se fija por configuración.</summary>
    public bool TelefonoObligatorio { get; set; }
}
