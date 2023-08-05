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

        string json = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "teste.json"));

        var load = JsonConvert.DeserializeObject<LoadData[]>(json);

        using MusicTrainner analizer = new();

        List<Task<Track>> tasks = new();
        foreach (var item in load)
        {
            var task = client.GetTrackAsync(item.Id);
            task.ContinueWith((task, obj) =>
            {
                if (task.Result.Restrictions != null)
                    return; 

                analizer.AddToTrainning(task.Result, item.Genres);
            }, null);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks.ToArray());
        await analizer.ProccessAsync();
        analizer.Trainnig();

        string outputFile = Path.Combine(Environment.CurrentDirectory, @".\Resources\Output.mma");

        await analizer.SaveToFileAsync(outputFile);

        var machineAnalizer = MusicAnalizer.ReadFile(outputFile);

        Console.Clear();
        Console.Write("Escreva o Json de músicas para testar: ");
        json = Console.ReadLine()!;
        load = JsonConvert.DeserializeObject<LoadData[]>(json);

        foreach (var item in load)
        {
            var track = await client.GetTrackAsync(item.Id);
            var rst = await machineAnalizer.GetGenreAsync(track);
        }
    }
}

public class LoadData
{
    public string Id { get; set; }
    public string[] Genres { get; set; }
}