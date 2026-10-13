using Cad.Contratos.Sos;
using Cad.Movil.Servicios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Cad.Movil.VistasModelo;

public enum EstadoSos
{
    Listo,
    ObteniendoUbicacion,
    Enviando,
    Registrado,
    NoConfirmado,
    Rechazado
}

/// <summary>
/// HU07/HU08: SOS con una sola activación, sin formularios ni confirmación (RNF06).
/// "SOS registrado" solo se muestra cuando responde el servidor; si no responde, se reintenta con el
/// mismo IdSolicitud para no duplicar el caso (RNF08).
/// </summary>
public sealed partial class SosViewModel(
    ApiCliente api, UbicacionServicio ubicacion, SosPendienteServicio pendiente, SesionServicio sesion) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnCurso), nameof(TextoBoton), nameof(HayResultado), nameof(Registrado),
        nameof(PuedeReintentar), nameof(Titulo))]
    private EstadoSos estado = EstadoSos.Listo;

    [ObservableProperty]
    private string detalle = "";

    public bool EnCurso => Estado is EstadoSos.ObteniendoUbicacion or EstadoSos.Enviando;
    public bool HayResultado => Estado is EstadoSos.Registrado or EstadoSos.NoConfirmado or EstadoSos.Rechazado;
    public bool Registrado => Estado == EstadoSos.Registrado;
    public bool PuedeReintentar => Estado == EstadoSos.NoConfirmado;

    public string TextoBoton => Estado switch
    {
        EstadoSos.ObteniendoUbicacion => "Ubicando…",
        EstadoSos.Enviando => "Enviando…",
        _ => "Pulsa para pedir ayuda"
    };

    public string Titulo => Estado switch
    {
        EstadoSos.Registrado => "SOS registrado",
        EstadoSos.NoConfirmado => "SOS no confirmado",
        EstadoSos.Rechazado => "No se pudo registrar el SOS",
        _ => ""
    };

    /// <summary>Al abrir la pantalla: si quedó un SOS sin confirmar, se ofrece reintentarlo.</summary>
    [RelayCommand]
    private async Task CargarPendienteAsync()
    {
        if (EnCurso || sesion.Actual is null || await pendiente.ObtenerAsync(sesion.Actual.UsuarioId) is null)
            return;

        Estado = EstadoSos.NoConfirmado;
        Detalle = "Tienes un SOS que el servidor no llegó a confirmar. Reintenta: no se duplicará.";
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task EmitirAsync()
    {
        if (sesion.Actual is not { } actual)
        {
            await Navegacion.IrAlLoginAsync();
            return;
        }

        // Un SOS sin confirmar se reenvía tal cual, con el mismo IdSolicitud y la misma ubicación.
        var solicitud = await pendiente.ObtenerAsync(actual.UsuarioId);
        if (solicitud is null)
        {
            Estado = EstadoSos.ObteniendoUbicacion;
            var posicion = await ubicacion.ObtenerPosicionActualAsync();
            // Android siempre informa la precisión del proveedor de ubicación; 0 solo si no la diera.
            var dto = posicion is null
                ? null
                : new UbicacionDto(posicion.Latitude, posicion.Longitude, posicion.Accuracy ?? 0, posicion.Timestamp);

            solicitud = new EmitirSosSolicitud(Guid.NewGuid(), dto);
            await pendiente.GuardarAsync(actual.UsuarioId, solicitud);
        }

        Estado = EstadoSos.Enviando;
        var respuesta = await api.EmitirSosAsync(solicitud);

        if (respuesta.EsExito)
        {
            pendiente.Borrar();
            var sos = respuesta.Valor!;
            Estado = EstadoSos.Registrado;
            Detalle = $"Registrado a las {sos.AceptadoEn.ToLocalTime():HH:mm:ss}, " +
                      (sos.ConUbicacion ? "con tu ubicación actual." : "con ubicación no disponible.");
            return;
        }

        switch (respuesta.Estado)
        {
            case 401:
                // La cuenta se desactivó o el token venció: el SOS queda guardado para reenviarlo tras entrar.
                Estado = EstadoSos.NoConfirmado;
                Detalle = respuesta.Error ?? "";
                sesion.Cerrar();
                await Shell.Current.DisplayAlertAsync("Sesión terminada",
                    "Vuelve a iniciar sesión. Tu SOS quedó guardado y podrás reenviarlo sin duplicarlo.", "Aceptar");
                await Navegacion.IrAlLoginAsync();
                break;

            case >= 400 and < 500:
                // El servidor rechazó el contenido: reintentarlo igual no serviría.
                pendiente.Borrar();
                Estado = EstadoSos.Rechazado;
                Detalle = respuesta.Error ?? "";
                break;

            default:
                // Sin respuesta o error del servidor: no se sabe si se registró; el reintento es seguro.
                Estado = EstadoSos.NoConfirmado;
                Detalle = $"{respuesta.Error} Reintenta: el SOS no se duplicará.";
                break;
        }
    }
}
