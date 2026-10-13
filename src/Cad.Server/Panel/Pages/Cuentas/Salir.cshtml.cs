using Cad.Server.Compartido.Seguridad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cad.Server.Panel.Pages.Cuentas;

/// <summary>Cierra la sesión del panel. Solo por POST, con token antifalsificación.</summary>
public sealed class SalirModel : PageModel
{
    public IActionResult OnGet() => LocalRedirect("/");

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(Politicas.EsquemaPanel);
        return LocalRedirect("/cuentas/login");
    }
}
