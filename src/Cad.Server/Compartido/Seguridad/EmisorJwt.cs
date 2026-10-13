using System.Text;
using Cad.Server.Persistencia.Identidad;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cad.Server.Compartido.Seguridad;

public sealed class EmisorJwt(IOptions<OpcionesJwt> opciones, TimeProvider reloj)
{
    public const string ClaimRol = "role";

    /// <summary>Sello de seguridad de Identity; si cambia, el token deja de ser válido (revocación, DP03).</summary>
    public const string ClaimSello = "sello";

    private readonly JsonWebTokenHandler _manejador = new();

    public (string Token, DateTimeOffset ExpiraEn) Emitir(Usuario usuario, string rol)
    {
        var o = opciones.Value;
        var ahora = reloj.GetUtcNow();
        var expiraEn = ahora.AddMinutes(o.MinutosVida);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = o.Emisor,
            Audience = o.Audiencia,
            IssuedAt = ahora.UtcDateTime,
            NotBefore = ahora.UtcDateTime,
            Expires = expiraEn.UtcDateTime,
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [ClaimRol] = rol,
                [ClaimSello] = usuario.SecurityStamp ?? ""
            },
            SigningCredentials = new SigningCredentials(CrearClave(o), SecurityAlgorithms.HmacSha256)
        };

        return (_manejador.CreateToken(descriptor), expiraEn);
    }

    public static SymmetricSecurityKey CrearClave(OpcionesJwt o) => new(Encoding.UTF8.GetBytes(o.ClaveFirma));
}
