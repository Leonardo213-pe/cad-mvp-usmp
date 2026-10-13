using System.ComponentModel.DataAnnotations;

namespace Cad.Server.Compartido.Seguridad;

public sealed class OpcionesJwt
{
    public const string Seccion = "Jwt";

    /// <summary>Clave HMAC de al menos 256 bits. Solo por variable de entorno Jwt__ClaveFirma o user-secrets.</summary>
    [Required, MinLength(32)]
    public string ClaveFirma { get; set; } = "";

    [Required]
    public string Emisor { get; set; } = "";

    [Required]
    public string Audiencia { get; set; } = "";

    /// <summary>Vida del token; no hay token de refresco (sección 9.6).</summary>
    [Range(5, 1440)]
    public int MinutosVida { get; set; } = 480;
}
