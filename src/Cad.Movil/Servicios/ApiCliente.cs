using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cad.Contratos.Cuentas;
using Cad.Contratos.Sos;

namespace Cad.Movil.Servicios;

/// <summary>Cliente REST del Servidor CAD. Todas las escrituras van por aquí (ADR-04).</summary>
public sealed class ApiCliente(HttpClient http, ConfiguracionServidor configuracion, SesionServicio sesion)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Task<RespuestaApi<RegistroCiudadanoRespuesta>> RegistrarCiudadanoAsync(
        RegistroCiudadanoSolicitud solicitud, CancellationToken ct = default) =>
        PostAsync<RegistroCiudadanoSolicitud, RegistroCiudadanoRespuesta>("api/cuentas/registro", solicitud, autenticado: false, ct);

    public Task<RespuestaApi<TokenRespuesta>> IniciarSesionAsync(TokenSolicitud solicitud, CancellationToken ct = default) =>
        PostAsync<TokenSolicitud, TokenRespuesta>("api/auth/token", solicitud, autenticado: false, ct);

    /// <summary>HU07: 201 si el SOS es nuevo, 200 si el IdSolicitud ya se había registrado.</summary>
    public Task<RespuestaApi<SosEmitidoRespuesta>> EmitirSosAsync(EmitirSosSolicitud solicitud, CancellationToken ct = default) =>
        PostAsync<EmitirSosSolicitud, SosEmitidoRespuesta>("api/sos", solicitud, autenticado: true, ct);

    private async Task<RespuestaApi<TRespuesta>> PostAsync<TSolicitud, TRespuesta>(
        string ruta, TSolicitud cuerpo, bool autenticado, CancellationToken ct)
    {
        if (!Uri.TryCreate(configuracion.Url.TrimEnd('/') + "/", UriKind.Absolute, out var baseUri))
            return RespuestaApi<TRespuesta>.Fallo("La dirección del servidor no es válida.");

        using var mensaje = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, ruta))
        {
            Content = JsonContent.Create(cuerpo, options: Json)
        };

        if (autenticado)
        {
            if (sesion.Actual is null)
                return RespuestaApi<TRespuesta>.Fallo("Tu sesión terminó. Vuelve a iniciar sesión.", 401);
            mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Actual.Token);
        }

        try
        {
            using var respuesta = await http.SendAsync(mensaje, ct);
            var estado = (int)respuesta.StatusCode;

            if (respuesta.IsSuccessStatusCode)
            {
                var valor = await respuesta.Content.ReadFromJsonAsync<TRespuesta>(Json, ct);
                return valor is null
                    ? RespuestaApi<TRespuesta>.Fallo("El servidor devolvió una respuesta vacía.", estado)
                    : RespuestaApi<TRespuesta>.Exito(valor, estado);
            }

            if (estado == 401 && autenticado)
                return RespuestaApi<TRespuesta>.Fallo("Tu sesión terminó. Vuelve a iniciar sesión.", estado);

            var problema = await LeerProblemaAsync(respuesta, ct);
            var texto = problema?.Detail ?? problema?.Title ?? $"El servidor respondió con el código {estado}.";
            return RespuestaApi<TRespuesta>.Fallo(texto, estado, problema?.Errors);
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
