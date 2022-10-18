namespace MonkeyFinder.ViewModel;

[QueryProperty(nameof(Monkey), "Monkey")]
public partial class MonkeyDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    Monkey monkey;

    readonly IMap map;
    
    public MonkeyDetailsViewModel(IMap map)
    {
        this.map = map;
    }

    [RelayCommand]
    async Task OpenMapAsync()
    {
        try
        {
            await map.OpenAsync(monkey.Latitude, monkey.Longitude,
                new MapLaunchOptions
                {
                    Name = monkey.Name,
                    NavigationMode = NavigationMode.None
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            await Shell.Current.DisplayAlert("Error", "Unable to open map.", "OK");
        }
    }
}
