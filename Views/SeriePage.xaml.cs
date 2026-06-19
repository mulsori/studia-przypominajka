using Przypominajka.Models;
using Przypominajka.ViewModels;

namespace Przypominajka.Views;

public partial class SeriePage : ContentPage
{
    private readonly SerieViewModel _vm;

    public SeriePage(SerieViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.WczytajCommand.ExecuteAsync(null);
    }

    // Switch jest dwukierunkowy – obsługujemy przez code-behind żeby uniknąć pętli
    private async void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        if (sender is Switch sw && sw.BindingContext is Seria seria)
            await _vm.PrzepnijAktywnoscCommand.ExecuteAsync(seria);
    }
}
