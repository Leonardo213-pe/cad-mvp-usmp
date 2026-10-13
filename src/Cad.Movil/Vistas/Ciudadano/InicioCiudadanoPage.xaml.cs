using Cad.Movil.VistasModelo;

namespace Cad.Movil.Vistas.Ciudadano;

public partial class InicioCiudadanoPage : ContentPage
{
    private readonly InicioCiudadanoViewModel _viewModel;
    private Window? _ventana;

    public InicioCiudadanoPage(InicioCiudadanoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Refrescar();
        Preparar();
        _viewModel.Sos.CargarPendienteCommand.Execute(null);

        // Al volver de los ajustes del teléfono se actualiza el estado sin volver a preguntar.
        _ventana = Window;
        if (_ventana is not null)
            _ventana.Resumed += AlReanudar;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_ventana is not null)
            _ventana.Resumed -= AlReanudar;
        _ventana = null;
    }

    private void AlReanudar(object? sender, EventArgs e) => Preparar();

    // El diálogo de permisos de Android también reanuda la ventana: no se lanza otra comprobación en paralelo.
    private void Preparar()
    {
        if (!_viewModel.PrepararCommand.IsRunning && !_viewModel.EjecutarAccionCommand.IsRunning)
            _viewModel.PrepararCommand.Execute(null);
    }
}
