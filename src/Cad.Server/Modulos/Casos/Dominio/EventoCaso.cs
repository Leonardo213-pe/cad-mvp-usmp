namespace Cad.Server.Modulos.Casos.Dominio;

public static class TiposEventoCaso
{
    public const string SosEmitido = "SosEmitido";
}

/// <summary>
/// Actuación registrada sobre un caso. Solo se inserta: nunca se modifica ni se borra.
/// Es la base del historial (HU27) y del cálculo de tiempos (HU28).
/// </summary>
public sealed class EventoCaso
{
    public const int LongitudMaximaTipo = 40;

    private EventoCaso()
    {
    }

    public Guid Id { get; private set; }
    public Guid CasoId { get; private set; }
    public string Tipo { get; private set; } = "";
    public Guid ActorId { get; private set; }
    public VisibilidadEvento Visibilidad { get; private set; }

    /// <summary>Datos propios del evento en JSON; nunca coordenadas ni datos personales.</summary>
    public string? Datos { get; private set; }

    public DateTimeOffset OcurridoEn { get; private set; }

    public static EventoCaso Crear(
        Guid casoId, string tipo, Guid actorId, VisibilidadEvento visibilidad, DateTimeOffset ahora, string? datos = null)
    {
        if (string.IsNullOrWhiteSpace(tipo) || tipo.Length > LongitudMaximaTipo)
            throw new ArgumentException("Tipo de evento inválido.", nameof(tipo));

        return new EventoCaso
        {
            Id = Guid.CreateVersion7(ahora),
            CasoId = casoId,
            Tipo = tipo,
            ActorId = actorId,
            Visibilidad = visibilidad,
            Datos = datos,
            OcurridoEn = ahora
        };
    }
}
