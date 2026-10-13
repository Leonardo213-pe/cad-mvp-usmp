namespace Cad.Contratos.TiempoReal;

/// <summary>Nombres compartidos entre el Hub, el panel y la app (sección 3.9).</summary>
public static class TiempoRealCad
{
    public const string RutaHub = "/hubs/cad";

    /// <summary>Servidor → grupo "operadores": se registró un SOS nuevo (HU11).</summary>
    public const string EventoSosNuevo = "SosNuevo";

    /// <summary>Panel → servidor: el SOS ya se muestra en la sección urgente (medición de RNF02).</summary>
    public const string MetodoPresentado = "Presentado";
}
