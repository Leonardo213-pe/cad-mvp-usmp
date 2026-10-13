using Microsoft.AspNetCore.Identity;

namespace Cad.Server.Persistencia.Identidad;

/// <summary>Cuenta de cualquier rol (ASP.NET Core Identity). El identificador de inicio de sesión es el email.</summary>
public sealed class Usuario : IdentityUser<Guid>
{
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreadoEn { get; set; }
}
