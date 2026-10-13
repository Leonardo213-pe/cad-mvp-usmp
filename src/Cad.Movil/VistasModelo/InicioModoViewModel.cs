using Cad.Movil.Servicios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cad.Movil.VistasModelo;

/// <summary>Pantalla principal de cada modo; sus funciones llegan con las HU siguientes.</summary>
public partial class InicioModoViewModel(SesionServicio sesion) : ObservableObject
{
    public string Email => sesion.Actual?.Email ?? "";

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        var confirmar = await Shell.Current.DisplayAlertAsync("Cerrar sesión", "¿Quieres cerrar tu sesión?", "Cerrar sesión", "Cancelar");
        if (!confirmar)
            return;

        sesion.Cerrar();
        await Navegacion.IrAlLoginAsync();
    }

    public void Refrescar() => OnPropertyChanged(nameof(Email));
}
