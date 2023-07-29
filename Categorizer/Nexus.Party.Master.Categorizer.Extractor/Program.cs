using Microsoft.Extensions.Configuration;
using Nexus.Spotify.Client;

public class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.Development.json"), true)
            .Build();

        using SpotifyClient client = await Utils.GetConsoleClientAsync(config);




    }


    public class LoadData
    {
        public string Id { get; set; }
        public string[] Genres { get; set; }
    }
}