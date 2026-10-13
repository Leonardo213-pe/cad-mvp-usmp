using Cad.Contratos.Cuentas;
using Cad.Movil.Servicios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cad.Movil.VistasModelo;

/// <summary>HU01. La validación aquí es solo de usabilidad: el servidor siempre revalida.</summary>
public sealed partial class RegistroViewModel(ApiCliente api, SesionServicio sesion) : ObservableObject
{
    [ObservableProperty]
    private string nombreCompleto = "";

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string telefono = "";

    [ObservableProperty]
    private string contrasena = "";

    [ObservableProperty]
    private string confirmacion = "";

    [ObservableProperty]
    private string? errorNombre;

    [ObservableProperty]
    private string? errorEmail;

    [ObservableProperty]
    private string? errorTelefono;

    [ObservableProperty]
    private string? errorContrasena;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayError))]
    private string? mensajeError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoOcupado))]
    private bool ocupado;

    public bool HayError => !string.IsNullOrEmpty(MensajeError);
    public bool NoOcupado => !Ocupado;

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        LimpiarErrores();
        if (Contrasena != Confirmacion)
        {
            ErrorContrasena = "Las contraseñas no coinciden.";
            return;
        }

        Ocupado = true;
        try
        {
            var solicitud = new RegistroCiudadanoSolicitud(
                NombreCompleto.Trim(), Email.Trim(), string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim(), Contrasena);

            var registro = await api.RegistrarCiudadanoAsync(solicitud);
            if (!registro.EsExito)
            {
                ErrorNombre = registro.ErrorDe("nombreCompleto");
                ErrorEmail = registro.ErrorDe("email");
                ErrorTelefono = registro.ErrorDe("telefono");
                ErrorContrasena = registro.ErrorDe("contrasena");
                MensajeError = registro.Error;
                return;
            }

            // Cuenta creada: se inicia sesión directamente con las mismas credenciales.
            var token = await api.IniciarSesionAsync(new TokenSolicitud(solicitud.Email, Contrasena));
            if (!token.EsExito)
            {
                await Shell.Current.DisplayAlertAsync("Cuenta creada", "Ya puedes iniciar sesión con tu email y contraseña.", "Aceptar");
                await Shell.Current.GoToAsync("..");
                return;
            }

            await sesion.GuardarAsync(token.Valor!, solicitud.Email);
            await Navegacion.IrAlModoAsync(token.Valor!.Rol);
        }
        finally
        {
            Ocupado = false;
        }
    }

    private void LimpiarErrores()
    {
        ErrorNombre = ErrorEmail = ErrorTelefono = ErrorContrasena = MensajeError = null;
    }
}
