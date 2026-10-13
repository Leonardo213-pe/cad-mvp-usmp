namespace Cad.Contratos.Sos;

/// <summary>
/// SOS tal como lo ve el operador en la sección urgente: llega en el evento SosNuevo y en
/// GET /api/central/sos-pendientes (resincronización al abrir o reconectar el panel).
/// </summary>
/// <param name="Ubicacion">Nula si el SOS se registró con "Ubicación no disponible".</param>
public sealed record SosCentralDto(
    Guid CasoId,
    string Estado,
    DateTimeOffset AceptadoEn,
    string Ciudadano,
    string? Telefono,
    UbicacionDto? Ubicacion);
