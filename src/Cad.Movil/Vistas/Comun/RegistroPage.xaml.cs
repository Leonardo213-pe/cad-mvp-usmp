using Cad.Movil.VistasModelo;

namespace Cad.Movil.Vistas.Comun;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(RegistroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
