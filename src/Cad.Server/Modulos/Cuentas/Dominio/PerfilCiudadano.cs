namespace Cad.Server.Modulos.Cuentas.Dominio;

/// <summary>Datos propios del ciudadano; extensión uno a uno de su cuenta.</summary>
public sealed class PerfilCiudadano
{
    public const int LongitudMaximaNombre = 150;
    public const int LongitudMaximaTelefono = 16;

    private PerfilCiudadano()
    {
    }

    public Guid UsuarioId { get; private set; }
    public string NombreCompleto { get; private set; } = "";

    /// <summary>Su obligatoriedad depende de DP01 (configurable).</summary>
    public string? Telefono { get; private set; }

    public DateTimeOffset RegistradoEn { get; private set; }

    public static PerfilCiudadano Crear(Guid usuarioId, string nombreCompleto, string? telefono, DateTimeOffset ahora)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("El perfil necesita una cuenta.", nameof(usuarioId));
        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Trim().Length > LongitudMaximaNombre)
            throw new ArgumentException("Nombre completo inválido.", nameof(nombreCompleto));

        return new PerfilCiudadano
        {
            UsuarioId = usuarioId,
            NombreCompleto = nombreCompleto.Trim(),
            Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim(),
            RegistradoEn = ahora
        };
    }
}
