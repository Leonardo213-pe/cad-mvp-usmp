namespace Cad.Contratos.Cuentas;

/// <summary>Cuerpo de POST /api/auth/token (HU02).</summary>
public sealed record TokenSolicitud(string Email, string Contrasena);

public sealed record TokenRespuesta(string AccessToken, DateTimeOffset ExpiraEn, Guid UsuarioId, string Rol);
