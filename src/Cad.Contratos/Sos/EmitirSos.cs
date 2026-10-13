namespace Cad.Contratos.Sos;

/// <summary>
/// Cuerpo de POST /api/sos (HU07/HU08). La app genera IdSolicitud una vez por pulsación y lo reutiliza
/// en cada reintento: el servidor nunca crea dos SOS con el mismo IdSolicitud (RNF08).
/// </summary>
/// <param name="Ubicacion">Nula cuando la ubicación actual no está disponible (HU08).</param>
public sealed record EmitirSosSolicitud(Guid IdSolicitud, UbicacionDto? Ubicacion);

/// <summary>Posición obtenida por el dispositivo, con su precisión y la hora en que se obtuvo.</summary>
public sealed record UbicacionDto(double Latitud, double Longitud, double PrecisionMetros, DateTimeOffset ObtenidaEn);

/// <summary>Respuesta de POST /api/sos: 201 si se creó, 200 si ya existía (reintento).</summary>
public sealed record SosEmitidoRespuesta(Guid CasoId, string Estado, DateTimeOffset AceptadoEn, bool ConUbicacion);
