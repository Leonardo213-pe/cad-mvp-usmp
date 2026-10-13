namespace Cad.Contratos.Salud;

/// <summary>Respuesta de GET /salud. No consulta la base de datos (ADR-05).</summary>
public sealed record EstadoSaludDto(string Estado, DateTimeOffset Hora);
