using Przypominajka.ViewModels;

namespace Przypominajka.Views;

public partial class PowiadomieniaPage : ContentPage
{
    private readonly PowiadomieniaViewModel _vm;

    public PowiadomieniaPage(PowiadomieniaViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.WczytajCommand.ExecuteAsync(null);
    }
}
