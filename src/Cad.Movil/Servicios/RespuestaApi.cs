namespace Cad.Movil.Servicios;

/// <summary>Resultado de una llamada al servidor: valor, o mensaje general y errores por campo.</summary>
public sealed class RespuestaApi<T>
{
    private static readonly IReadOnlyDictionary<string, string[]> SinErrores = new Dictionary<string, string[]>();

    private RespuestaApi(T? valor, int? estado, string? error, IReadOnlyDictionary<string, string[]>? erroresCampo)
    {
        Valor = valor;
        Estado = estado;
        Error = error;
        ErroresCampo = erroresCampo ?? SinErrores;
    }

    public T? Valor { get; }

    /// <summary>Código HTTP; nulo si no hubo respuesta del servidor.</summary>
    public int? Estado { get; }

    public string? Error { get; }
    public IReadOnlyDictionary<string, string[]> ErroresCampo { get; }
    public bool EsExito => Error is null;

    public string? ErrorDe(string campo) =>
        ErroresCampo.TryGetValue(campo, out var mensajes) ? string.Join(" ", mensajes) : null;

    public static RespuestaApi<T> Exito(T valor, int estado) => new(valor, estado, null, null);

    public static RespuestaApi<T> Fallo(string error, int? estado = null,
        IReadOnlyDictionary<string, string[]>? erroresCampo = null) => new(default, estado, error, erroresCampo);
}
