namespace Cad.Server.Modulos.Despacho.Dominio;

/// <summary>
/// Estado operativo y última posición del sereno. Solo se guarda la última posición (sección 9.1).
/// La disponibilidad no se almacena: se calcula como operativo y sin asignación vigente (HU19).
/// </summary>
public sealed class EstadoSereno
{
    private EstadoSereno()
    {
    }

    public Guid UsuarioId { get; private set; }
    public bool Operativo { get; private set; }
    public double? Latitud { get; private set; }
    public double? Longitud { get; private set; }
    public double? PrecisionMetros { get; private set; }
    public DateTimeOffset? PosicionObtenidaEn { get; private set; }
    public DateTimeOffset ActualizadoEn { get; private set; }

    public static EstadoSereno Crear(Guid usuarioId, DateTimeOffset ahora) => new()
    {
        UsuarioId = usuarioId,
        Operativo = false,
        ActualizadoEn = ahora
    };
}
