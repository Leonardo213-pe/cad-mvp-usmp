using Cad.Contratos.Cuentas;
using Cad.Server.Compartido.Seguridad;
using Cad.Server.Modulos.Cuentas.Aplicacion;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cad.Server.Panel.Pages.Cuentas;

/// <summary>HU02 en el panel: cookie HttpOnly para operador y administrador (sección 9.6).</summary>
public sealed class LoginModel(ServicioCuentas cuentas) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Contrasena { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? Error { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validacion = await cuentas.ValidarCredencialesAsync(Email, Contrasena);
        if (!validacion.EsExito)
        {
            Error = validacion.Error!.Mensaje;
            return Page();
        }

        var (usuario, rol) = validacion.Valor!;
        if (rol is not (Roles.Operador or Roles.Administrador))
        {
            Error = "Las cuentas de ciudadano y de sereno se usan desde la app móvil.";
            return Page();
        }

        await HttpContext.SignInAsync(Politicas.EsquemaPanel, ConfiguracionSeguridad.CrearPrincipalPanel(usuario, rol));

        var destino = rol == Roles.Operador ? "/central" : "/";
        return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : destino);
    }
}
