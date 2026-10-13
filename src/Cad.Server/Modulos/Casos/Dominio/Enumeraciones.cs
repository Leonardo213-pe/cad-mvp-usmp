namespace Cad.Server.Modulos.Casos.Dominio;

/// <summary>Un caso nace de un SOS o de un reporte; ambos comparten el ciclo de despacho (sección 5.8).</summary>
public enum OrigenCaso
{
    Sos,
    Reporte
}

/// <summary>Estados de la figura 8. Solo se usan las transiciones que definen los criterios de aceptación.</summary>
public enum EstadoCaso
{
    Pendiente,
    Recibido,
    EnRevision,
    Asignada,
    EnCamino,
    EnAtencion,
    Cerrada,
    Cancelada,
    Derivada,
    Descartada
}

public enum VisibilidadEvento
{
    /// <summary>Puede mostrarse al ciudadano titular del caso (según DP05).</summary>
    Publica,

    /// <summary>Solo para roles autorizados de la central (RNF12).</summary>
    Interna
}
