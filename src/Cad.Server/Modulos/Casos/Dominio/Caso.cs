namespace Cad.Server.Modulos.Casos.Dominio;

/// <summary>Caso común a SOS y reportes. Su historial se guarda aparte, en <see cref="EventoCaso"/>.</summary>
public sealed class Caso
{
    private Caso()
    {
    }

    public Guid Id { get; private set; }
    public OrigenCaso Origen { get; private set; }
    public EstadoCaso Estado { get; private set; }
    public Guid CiudadanoId { get; private set; }

    /// <summary>Generado por la app; único por ciudadano para que un reintento no duplique el caso (RNF08).</summary>
    public Guid IdSolicitud { get; private set; }

    // Coordenadas, precisión y hora de obtención: todas nulas o todas presentes.
    public double? Latitud { get; private set; }
    public double? Longitud { get; private set; }
    public double? PrecisionMetros { get; private set; }
    public DateTimeOffset? UbicacionObtenidaEn { get; private set; }

    /// <summary>Hora en que el servidor aceptó el caso; base de la medición de RNF02.</summary>
    public DateTimeOffset CreadoEn { get; private set; }

    /// <summary>Token de concurrencia optimista (xmin de PostgreSQL) para las transiciones futuras.</summary>
    public uint Version { get; private set; }

    public bool TieneUbicacion => Latitud is not null;

    /// <summary>HU07/HU08: un SOS nace en "Pendiente", con o sin ubicación, junto con su evento de historial.</summary>
    public static (Caso Caso, EventoCaso Evento) CrearSos(
        Guid id, Guid ciudadanoId, Guid idSolicitud, UbicacionCaso? ubicacion, DateTimeOffset ahora)
    {
        if (id == Guid.Empty || ciudadanoId == Guid.Empty || idSolicitud == Guid.Empty)
            throw new ArgumentException("El SOS necesita identificador, ciudadano e IdSolicitud.");
        if (ubicacion is not null && !ubicacion.EsValida())
            throw new ArgumentException("La ubicación del SOS está fuera de rango.", nameof(ubicacion));

        var caso = new Caso
        {
            Id = id,
            Origen = OrigenCaso.Sos,
            Estado = EstadoCaso.Pendiente,
            CiudadanoId = ciudadanoId,
            IdSolicitud = idSolicitud,
            Latitud = ubicacion?.Latitud,
            Longitud = ubicacion?.Longitud,
            PrecisionMetros = ubicacion?.PrecisionMetros,
            UbicacionObtenidaEn = ubicacion?.ObtenidaEn,
            CreadoEn = ahora
        };

        var evento = EventoCaso.Crear(
            caso.Id, TiposEventoCaso.SosEmitido, ciudadanoId, VisibilidadEvento.Publica, ahora,
            $$"""{"conUbicacion":{{(caso.TieneUbicacion ? "true" : "false")}}}""");

        return (caso, evento);
    }
}
