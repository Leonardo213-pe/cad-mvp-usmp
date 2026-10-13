using Cad.Movil.VistasModelo;

namespace Cad.Movil.Vistas.Ciudadano;

public partial class InicioCiudadanoPage : ContentPage
{
    private readonly InicioModoViewModel _viewModel;

    public InicioCiudadanoPage(InicioModoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Refrescar();
    }
}
