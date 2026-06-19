using Przypominajka.ViewModels;

namespace Przypominajka.Views;

public partial class ZadaniaPage : ContentPage
{
    private readonly ZadaniaViewModel _vm;

    public ZadaniaPage(ZadaniaViewModel vm)
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
