namespace Cad.Contratos.Cuentas;

/// <summary>Cuerpo de POST /api/cuentas/registro (HU01).</summary>
public sealed record RegistroCiudadanoSolicitud(string NombreCompleto, string Email, string? Telefono, string Contrasena);

public sealed record RegistroCiudadanoRespuesta(Guid UsuarioId, string Email);
