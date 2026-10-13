using Cad.Movil.VistasModelo;

namespace Cad.Movil.Vistas.Sereno;

public partial class InicioSerenoPage : ContentPage
{
    private readonly InicioModoViewModel _viewModel;

    public InicioSerenoPage(InicioModoViewModel viewModel)
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
