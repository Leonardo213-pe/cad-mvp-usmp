using Cad.Movil.Servicios;
using Cad.Movil.Vistas.Ciudadano;
using Cad.Movil.Vistas.Comun;
using Cad.Movil.Vistas.Sereno;
using Cad.Movil.VistasModelo;

namespace Cad.Movil;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddSingleton(SecureStorage.Default);
        builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(15) });
        builder.Services.AddSingleton<ConfiguracionServidor>();
        builder.Services.AddSingleton<SesionServicio>();
        builder.Services.AddSingleton<ApiCliente>();
        builder.Services.AddSingleton<UbicacionServicio>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistroViewModel>();
        builder.Services.AddTransient<InicioModoViewModel>();
        builder.Services.AddTransient<InicioCiudadanoViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistroPage>();
        builder.Services.AddTransient<InicioCiudadanoPage>();
        builder.Services.AddTransient<InicioSerenoPage>();

        return builder.Build();
    }
}
