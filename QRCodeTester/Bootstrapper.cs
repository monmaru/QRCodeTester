using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using QRCodeTester.Views;
using System.Windows;

namespace QRCodeTester
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            // moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }
    }
}