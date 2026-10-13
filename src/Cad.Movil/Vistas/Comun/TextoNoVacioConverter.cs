using System.Globalization;

namespace Cad.Movil.Vistas.Comun;

/// <summary>Muestra un control solo si el texto enlazado tiene contenido (errores por campo).</summary>
public sealed class TextoNoVacioConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !string.IsNullOrEmpty(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
