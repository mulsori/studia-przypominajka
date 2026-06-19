using Przypominajka.Views;

namespace Przypominajka;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Rejestracja tras dla stron formularzy (bez flyout)
        Routing.RegisterRoute(nameof(ZadanieFormPage), typeof(ZadanieFormPage));
        Routing.RegisterRoute(nameof(KategoriaFormPage), typeof(KategoriaFormPage));
        Routing.RegisterRoute(nameof(SeriaFormPage), typeof(SeriaFormPage));
    }
}
