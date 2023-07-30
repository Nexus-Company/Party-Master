using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nexus.Party.Master.Categorizer.Analizer;
using Nexus.Spotify.Client;
using Nexus.Spotify.Client.Models;

public class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.Development.json"), true)
            .Build();

        using SpotifyClient client = await Utils.GetConsoleClientAsync(config);

        string json = Console.ReadLine()!;

        var load = JsonConvert.DeserializeObject<LoadData[]>(json);

        using MusicTrainner analizer = new();

        foreach (var item in load)
        {
            var track = await client.GetTrackAsync(item.Id);
            await analizer.AddToTrainnigAsync(track, item.Genres);
        }

        analizer.Proccess();
        analizer.Trainnig();

        await analizer.SaveToFileAsync(Path.Combine(Environment.CurrentDirectory, "output.mma"));
    }
}


public class LoadData
{
    public string Id { get; set; }
    public string[] Genres { get; set; }
}