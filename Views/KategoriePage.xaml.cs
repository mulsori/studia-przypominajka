using Przypominajka.ViewModels;

namespace Przypominajka.Views;

public partial class KategoriePage : ContentPage
{
    private readonly KategorieViewModel _vm;

    public KategoriePage(KategorieViewModel vm)
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
