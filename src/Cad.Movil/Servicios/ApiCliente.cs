using System.Net.Http.Json;
using System.Text.Json;
using Cad.Contratos.Cuentas;

namespace Cad.Movil.Servicios;

/// <summary>Cliente REST del Servidor CAD. Todas las escrituras van por aquí (ADR-04).</summary>
public sealed class ApiCliente(HttpClient http, ConfiguracionServidor configuracion)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Task<RespuestaApi<RegistroCiudadanoRespuesta>> RegistrarCiudadanoAsync(
        RegistroCiudadanoSolicitud solicitud, CancellationToken ct = default) =>
        PostAsync<RegistroCiudadanoSolicitud, RegistroCiudadanoRespuesta>("api/cuentas/registro", solicitud, ct);

    public Task<RespuestaApi<TokenRespuesta>> IniciarSesionAsync(TokenSolicitud solicitud, CancellationToken ct = default) =>
        PostAsync<TokenSolicitud, TokenRespuesta>("api/auth/token", solicitud, ct);

    private async Task<RespuestaApi<TRespuesta>> PostAsync<TSolicitud, TRespuesta>(
        string ruta, TSolicitud cuerpo, CancellationToken ct)
    {
        if (!Uri.TryCreate(configuracion.Url.TrimEnd('/') + "/", UriKind.Absolute, out var baseUri))
            return RespuestaApi<TRespuesta>.Fallo("La dirección del servidor no es válida.");

        try
        {
            using var respuesta = await http.PostAsJsonAsync(new Uri(baseUri, ruta), cuerpo, Json, ct);
            var estado = (int)respuesta.StatusCode;

            if (respuesta.IsSuccessStatusCode)
            {
                var valor = await respuesta.Content.ReadFromJsonAsync<TRespuesta>(Json, ct);
                return valor is null
                    ? RespuestaApi<TRespuesta>.Fallo("El servidor devolvió una respuesta vacía.", estado)
                    : RespuestaApi<TRespuesta>.Exito(valor, estado);
            }

            var problema = await LeerProblemaAsync(respuesta, ct);
            var mensaje = problema?.Detail ?? problema?.Title ?? $"El servidor respondió con el código {estado}.";
            return RespuestaApi<TRespuesta>.Fallo(mensaje, estado, problema?.Errors);
        }
        catch (HttpRequestException)
        {
            return RespuestaApi<TRespuesta>.Fallo("No se pudo conectar con el servidor. Revisa la dirección y tu conexión.");
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return RespuestaApi<TRespuesta>.Fallo("El servidor no respondió a tiempo.");
        }
    }

    private static async Task<ProblemaApi?> LeerProblemaAsync(HttpResponseMessage respuesta, CancellationToken ct)
    {
        try
        {
            return await respuesta.Content.ReadFromJsonAsync<ProblemaApi>(Json, ct);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>ProblemDetails que devuelve el servidor ante un error.</summary>
    private sealed record ProblemaApi(string? Title, string? Detail, Dictionary<string, string[]>? Errors);
}
