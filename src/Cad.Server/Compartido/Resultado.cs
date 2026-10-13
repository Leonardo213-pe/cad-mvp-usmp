namespace Cad.Server.Compartido;

public enum TipoError
{
    Validacion,
    Conflicto,
    NoAutenticado,
    Prohibido
}

public sealed record Error(TipoError Tipo, string Codigo, string Mensaje, IDictionary<string, string[]>? Detalles = null);

/// <summary>Resultado de un caso de uso: valor o error de negocio, sin tipos HTTP.</summary>
public sealed class Resultado<T>
{
    private Resultado(T? valor, Error? error)
    {
        Valor = valor;
        Error = error;
    }

    public T? Valor { get; }
    public Error? Error { get; }
    public bool EsExito => Error is null;

    public static Resultado<T> Exito(T valor) => new(valor, null);

    public static implicit operator Resultado<T>(Error error) => new(default, error);
}
