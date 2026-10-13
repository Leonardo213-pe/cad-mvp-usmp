namespace Cad.Server.Compartido;

/// <summary>Traduce errores de aplicación a respuestas ProblemDetails.</summary>
public static class ErroresHttp
{
    public static IResult ARespuesta(this Error error) => error.Tipo switch
    {
        TipoError.Validacion => Results.ValidationProblem(
            error.Detalles ?? new Dictionary<string, string[]> { [error.Codigo] = [error.Mensaje] },
            title: error.Mensaje),
        TipoError.Conflicto => Results.Problem(error.Mensaje, statusCode: StatusCodes.Status409Conflict, title: error.Codigo),
        TipoError.NoAutenticado => Results.Problem(error.Mensaje, statusCode: StatusCodes.Status401Unauthorized, title: error.Codigo),
        TipoError.Prohibido => Results.Problem(error.Mensaje, statusCode: StatusCodes.Status403Forbidden, title: error.Codigo),
        _ => Results.Problem(error.Mensaje)
    };
}
