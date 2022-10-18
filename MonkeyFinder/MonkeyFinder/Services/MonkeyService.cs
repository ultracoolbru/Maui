using System.Net.Http.Json;

namespace MonkeyFinder.Services;

public class MonkeyService
{
    HttpClient client;

    public MonkeyService()
    {
        client = new HttpClient();
    }
    
    List<Monkey> monkeys = new();
    
    public async Task<List<Monkey>> GetMonkeysAsync()
    {
        if (monkeys.Any())
            return monkeys;

        var url = "https://raw.githubusercontent.com/jamesmontemagno/app-monkeys/master/MonkeysApp/monkeydata.json";

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            monkeys = await response.Content.ReadFromJsonAsync<List<Monkey>>(); 
        }
        

        return monkeys;
    }
}
