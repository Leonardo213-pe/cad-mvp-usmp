namespace Cad.Server.Modulos.Casos.Dominio;

/// <summary>
/// Solo para la prueba de RNF02 (tabla 23): tiempo desde que el servidor aceptó el SOS hasta el acuse
/// del panel. Es una cota superior del tiempo real de presentación. Se guarda el primer acuse por SOS.
/// </summary>
public sealed class MedicionPresentacion
{
    private MedicionPresentacion()
    {
    }

    public Guid CasoId { get; private set; }
    public Guid OperadorId { get; private set; }
    public DateTimeOffset AceptadoEn { get; private set; }
    public DateTimeOffset AcuseEn { get; private set; }
    public double Milisegundos { get; private set; }

    public static MedicionPresentacion Crear(Guid casoId, Guid operadorId, DateTimeOffset aceptadoEn, DateTimeOffset acuseEn) =>
        new()
        {
            CasoId = casoId,
            OperadorId = operadorId,
            AceptadoEn = aceptadoEn,
            AcuseEn = acuseEn,
            Milisegundos = (acuseEn - aceptadoEn).TotalMilliseconds
        };
}
