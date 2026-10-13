using Android.App;
using Android.Runtime;

namespace Cad.Movil;

// RNF09: en Release solo HTTPS. En Debug se permite HTTP para probar contra el servidor local.
#if DEBUG
[Application(UsesCleartextTraffic = true)]
#else
[Application(UsesCleartextTraffic = false)]
#endif
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
