using System.Data.SqlTypes;
using System.Linq;

using MonkeyFinder.Services;

namespace MonkeyFinder.ViewModel;

public partial class MonkeysViewModel : BaseViewModel
{
    readonly MonkeyService monkeyService;
    readonly IConnectivity connectivity;
    readonly IGeolocation geolocation;

    [ObservableProperty]
    bool isRefreshing = false;

    public ObservableCollection<Monkey> Monkeys { get; } = new();

    public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation)
    {
        Title = "Monkey Service";

        this.monkeyService = monkeyService;
        this.connectivity = connectivity;
        this.geolocation = geolocation;
    }

    [RelayCommand]
    async Task GetClosestMonkeyAsync()
    {
        if (IsBusy || !Monkeys.Any()) return;

        try
        {
            var location = await geolocation.GetLastKnownLocationAsync();

            location ??= await geolocation.GetLocationAsync(
                    new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });

            if (location is null) return;

            var first = Monkeys.OrderBy(m => location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Kilometers)).FirstOrDefault();

            if (first is null) return;

            await Shell.Current.DisplayAlert("Closest Monkey", $"{first.Name} in { first.Location}", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            await Shell.Current.DisplayAlert("Error", "Unable to get closest monkeys.", "OK");
        }

    }

    [RelayCommand]
    async Task GoToDetailsAsync(Monkey monkey)
    {
        if (monkey is null)
            return;

        await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
            new Dictionary<string, object>
            {
                {"Monkey", monkey }
            });
    }

    [RelayCommand]
    async Task GetMonkeysAsync()
    {
        if (IsBusy) return;

        try
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet Error", "Check your internet connection and try again.", "OK");
                return;
            }

            IsBusy = true;

            var monkeys = await monkeyService.GetMonkeysAsync();

            if (Monkeys.Any())
                Monkeys.Clear();

            foreach (var monkey in monkeys)
            {
                Monkeys.Add(monkey);
            }


        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            await Shell.Current.DisplayAlert("Error", "Unable to get monkeys.", "OK");
        }
        finally
        {
            IsBusy = false;
            isRefreshing = false;
        }
    }

    [RelayCommand]
    async Task ClearMonkeysAsync()
    {

        if (IsBusy) return;

        try
        {
            Monkeys.Clear();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            await Shell.Current.DisplayAlert("Error", "Unable to clear monkeys.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
        

}
