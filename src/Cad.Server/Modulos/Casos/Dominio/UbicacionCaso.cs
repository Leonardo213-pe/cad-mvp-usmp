namespace Cad.Server.Modulos.Casos.Dominio;

/// <summary>Posición reportada por el dispositivo. Sin PostGIS: latitud y longitud con validación de rango (ADR-03).</summary>
public sealed record UbicacionCaso(double Latitud, double Longitud, double PrecisionMetros, DateTimeOffset ObtenidaEn)
{
    public static bool LatitudValida(double valor) => double.IsFinite(valor) && valor is >= -90 and <= 90;

    public static bool LongitudValida(double valor) => double.IsFinite(valor) && valor is >= -180 and <= 180;

    public static bool PrecisionValida(double valor) => double.IsFinite(valor) && valor >= 0;

    public bool EsValida() => LatitudValida(Latitud) && LongitudValida(Longitud) && PrecisionValida(PrecisionMetros);
}
