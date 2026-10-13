using Cad.Movil.Servicios;

namespace Cad.Movil;

public partial class App : Application
{
    private readonly SesionServicio _sesion;

    public App(SesionServicio sesion)
    {
        InitializeComponent();
        _sesion = sesion;
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());

    protected override async void OnStart()
    {
        base.OnStart();

        // Si hay un token vigente guardado, se entra directo al modo de la cuenta.
        var sesion = await _sesion.CargarAsync();
        if (sesion is not null && Navegacion.RolUsaLaApp(sesion.Rol))
            await Navegacion.IrAlModoAsync(sesion.Rol);
    }
}
