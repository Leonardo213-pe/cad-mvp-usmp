using Microsoft.AspNetCore.Identity;

namespace Cad.Server.Persistencia.Identidad;

/// <summary>Mensajes de Identity en español para los errores que llegan al cliente.</summary>
public sealed class DescriptorErroresIdentidad : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email) =>
        new() { Code = nameof(DuplicateEmail), Description = "Ya existe una cuenta con ese email." };

    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = "Ya existe una cuenta con ese email." };

    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = nameof(InvalidEmail), Description = "El email no es válido." };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe incluir al menos un número." };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe incluir al menos una letra minúscula." };
}
