using Cad.Contratos.Cuentas;
using Cad.Movil.Servicios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cad.Movil.VistasModelo;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly ApiCliente _api;
    private readonly SesionServicio _sesion;
    private readonly ConfiguracionServidor _configuracion;

    public LoginViewModel(ApiCliente api, SesionServicio sesion, ConfiguracionServidor configuracion)
    {
        _api = api;
        _sesion = sesion;
        _configuracion = configuracion;
        servidorUrl = configuracion.Url;
    }

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string contrasena = "";

    [ObservableProperty]
    private string servidorUrl;

    [ObservableProperty]
    private bool mostrarServidor;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? mensajeError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoOcupado))]
    private bool ocupado;

    public bool HayError => !string.IsNullOrEmpty(MensajeError);
    public bool NoOcupado => !Ocupado;

    [RelayCommand]
    private void AlternarServidor() => MostrarServidor = !MostrarServidor;

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        MensajeError = null;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Contrasena))
        {
            MensajeError = "Ingresa tu email y tu contraseña.";
            return;
        }

        Ocupado = true;
        try
        {
            _configuracion.Url = ServidorUrl;
            var respuesta = await _api.IniciarSesionAsync(new TokenSolicitud(Email.Trim(), Contrasena));
            if (!respuesta.EsExito)
            {
                MensajeError = respuesta.Error;
                return;
            }

            var token = respuesta.Valor!;
            if (!Navegacion.RolUsaLaApp(token.Rol))
            {
                MensajeError = $"Las cuentas de {token.Rol.ToLowerInvariant()} se usan desde el panel web de central.";
                return;
            }

            await _sesion.GuardarAsync(token, Email.Trim());
            Contrasena = "";
            await Navegacion.IrAlModoAsync(token.Rol);
        }
        finally
        {
            Ocupado = false;
        }
    }

    [RelayCommand]
    private Task IrARegistroAsync() => Shell.Current.GoToAsync(Navegacion.Registro);
}
