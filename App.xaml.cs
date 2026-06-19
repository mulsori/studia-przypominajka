namespace Przypominajka;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell())
        {
            Title = "Przypominajka",
            Width = 1024,
            Height = 768,
            MinimumWidth = 800,
            MinimumHeight = 600
        };
}
