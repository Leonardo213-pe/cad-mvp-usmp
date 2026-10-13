using Cad.Movil.VistasModelo;

namespace Cad.Movil.Vistas.Comun;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
