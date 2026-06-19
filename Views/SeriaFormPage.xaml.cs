using Przypominajka.ViewModels;

namespace Przypominajka.Views;

public partial class SeriaFormPage : ContentPage, IQueryAttributable
{
    private readonly SeriaFormViewModel _vm;

    public SeriaFormPage(SeriaFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
        => _vm.ApplyQueryAttributes(query);

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.WczytajCommand.ExecuteAsync(null);
    }
}
