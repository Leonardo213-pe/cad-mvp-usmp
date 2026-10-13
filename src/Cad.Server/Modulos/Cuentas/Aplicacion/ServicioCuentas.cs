using System.Net.Mail;
using System.Text.RegularExpressions;
using Cad.Contratos.Cuentas;
using Cad.Server.Compartido;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Cuentas.Dominio;
using Cad.Server.Persistencia;
using Cad.Server.Persistencia.Identidad;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Cad.Server.Modulos.Cuentas.Aplicacion;

public sealed partial class ServicioCuentas(
    CadDbContext db,
    UserManager<Usuario> usuarios,
    EmisorJwt emisorJwt,
    TimeProvider reloj,
    IOptions<OpcionesCuentas> opciones)
{
    private const int LongitudMaximaEmail = 256;

    private static readonly Error EmailDuplicado =
        new(TipoError.Conflicto, "email_duplicado", "Ya existe una cuenta con ese email.");

    private static readonly Error CredencialesInvalidas =
        new(TipoError.NoAutenticado, "credenciales_invalidas", "Email o contraseña incorrectos.");

    /// <summary>HU01: crea cuenta, rol Ciudadano y perfil en una sola transacción.</summary>
    public async Task<Resultado<RegistroCiudadanoRespuesta>> RegistrarCiudadanoAsync(
        RegistroCiudadanoSolicitud solicitud, CancellationToken ct)
    {
        var errores = ValidarRegistro(solicitud);
        if (errores.Count > 0)
            return new Error(TipoError.Validacion, "registro_invalido", "Los datos de registro no son válidos.", errores);

        var ahora = reloj.GetUtcNow();
        var email = solicitud.Email.Trim();
        var usuario = new Usuario
        {
            Id = Guid.CreateVersion7(ahora),
            UserName = email,
            Email = email,
            CreadoEn = ahora
        };

        await using var transaccion = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var creacion = await usuarios.CreateAsync(usuario, solicitud.Contrasena);
            if (!creacion.Succeeded)
                return ErrorDeIdentidad(creacion);

            var asignacion = await usuarios.AddToRoleAsync(usuario, Roles.Ciudadano);
            if (!asignacion.Succeeded)
                throw new InvalidOperationException("No se pudo asignar el rol Ciudadano.");

            db.PerfilesCiudadano.Add(PerfilCiudadano.Crear(usuario.Id, solicitud.NombreCompleto, solicitud.Telefono, ahora));
            await db.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Dos registros simultáneos con el mismo email: el índice único decide (HU01).
            return EmailDuplicado;
        }

        return Resultado<RegistroCiudadanoRespuesta>.Exito(new RegistroCiudadanoRespuesta(usuario.Id, email));
    }

    /// <summary>HU02: valida credenciales y emite un JWT para la app.</summary>
    public async Task<Resultado<TokenRespuesta>> EmitirTokenAsync(TokenSolicitud solicitud, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Email) || string.IsNullOrEmpty(solicitud.Contrasena))
            return CredencialesInvalidas;

        var usuario = await usuarios.FindByEmailAsync(solicitud.Email.Trim());
        if (usuario is null || !await usuarios.CheckPasswordAsync(usuario, solicitud.Contrasena))
            return CredencialesInvalidas;

        if (!usuario.Activo)
            return new Error(TipoError.Prohibido, "cuenta_desactivada", "La cuenta está desactivada.");

        var rol = (await usuarios.GetRolesAsync(usuario)).SingleOrDefault();
        if (rol is null)
            return new Error(TipoError.Prohibido, "cuenta_sin_rol", "La cuenta no tiene un rol asignado.");

        var (token, expiraEn) = emisorJwt.Emitir(usuario, rol);
        return Resultado<TokenRespuesta>.Exito(new TokenRespuesta(token, expiraEn, usuario.Id, rol));
    }

    private Dictionary<string, string[]> ValidarRegistro(RegistroCiudadanoSolicitud s)
    {
        var errores = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(s.NombreCompleto))
            errores["nombreCompleto"] = ["El nombre completo es obligatorio."];
        else if (s.NombreCompleto.Trim().Length > PerfilCiudadano.LongitudMaximaNombre)
            errores["nombreCompleto"] = [$"El nombre completo admite hasta {PerfilCiudadano.LongitudMaximaNombre} caracteres."];

        if (string.IsNullOrWhiteSpace(s.Email))
            errores["email"] = ["El email es obligatorio."];
        else if (s.Email.Trim().Length > LongitudMaximaEmail || !MailAddress.TryCreate(s.Email.Trim(), out _))
            errores["email"] = ["El email no es válido."];

        if (string.IsNullOrWhiteSpace(s.Telefono))
        {
            if (opciones.Value.TelefonoObligatorio)
                errores["telefono"] = ["El teléfono es obligatorio."];
        }
        else if (!PatronTelefono().IsMatch(s.Telefono.Trim()))
        {
            errores["telefono"] = ["El teléfono debe tener entre 6 y 15 dígitos, con + opcional al inicio."];
        }

        if (string.IsNullOrEmpty(s.Contrasena))
            errores["contrasena"] = ["La contraseña es obligatoria."];

        return errores;
    }

    private static Error ErrorDeIdentidad(IdentityResult resultado)
    {
        if (resultado.Errors.Any(e => e.Code is nameof(IdentityErrorDescriber.DuplicateEmail)
                or nameof(IdentityErrorDescriber.DuplicateUserName)))
            return EmailDuplicado;

        var detalles = resultado.Errors
            .GroupBy(e => e.Code.StartsWith("Password", StringComparison.Ordinal) ? "contrasena" : "email")
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return new Error(TipoError.Validacion, "registro_invalido", "Los datos de registro no son válidos.", detalles);
    }

    [GeneratedRegex(@"^\+?[0-9]{6,15}$")]
    private static partial Regex PatronTelefono();
}
