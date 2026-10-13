using Cad.Contratos.Cuentas;

namespace Cad.Movil.Servicios;

/// <summary>Rutas del Shell. El modo de la app depende del rol de la cuenta (sección 3.2).</summary>
public static class Navegacion
{
    public const string Login = "login";
    public const string Registro = "registro";
    public const string Ciudadano = "ciudadano";
    public const string Sereno = "sereno";

    public static bool RolUsaLaApp(string rol) => rol is Roles.Ciudadano or Roles.Sereno;

    public static Task IrAlModoAsync(string rol) =>
        Shell.Current.GoToAsync(rol == Roles.Sereno ? $"//{Sereno}" : $"//{Ciudadano}");

    public static Task IrAlLoginAsync() => Shell.Current.GoToAsync($"//{Login}");
}
