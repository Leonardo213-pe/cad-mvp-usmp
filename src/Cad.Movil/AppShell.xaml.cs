using Cad.Movil.Servicios;
using Cad.Movil.Vistas.Comun;

namespace Cad.Movil;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Navegacion.Registro, typeof(RegistroPage));
    }
}
