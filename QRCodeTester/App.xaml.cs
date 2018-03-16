using System.Windows;
using Reactive.Bindings;

namespace QRCodeTester
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            UIDispatcherScheduler.Initialize();
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}