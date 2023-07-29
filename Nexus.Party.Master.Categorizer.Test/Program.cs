using Microsoft.Extensions.Configuration;
using Nexus.Party.Master.Categorizer.Analizer;
using Nexus.Spotify.Client;
using System.Diagnostics;
using System.Net;

public class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            .Build();

        var spotifyConfig = config.GetSection("Spotify");

        string[] scopes = spotifyConfig.GetSection("Scopes")
            .GetChildren()
            .Select(x => x.Value)
            .ToArray()!;

        string clientId = spotifyConfig["ClientId"]!,
           secret = spotifyConfig["Secret"]!,
           link = OAuthCredential.GenerateOAuthLink(clientId, null, scopes);

        Process.Start(new ProcessStartInfo(link)
        {
            UseShellExecute = true
        });

        var credential = await OAuthCredential
            .GetCredentialAsync(clientId, secret, GetCode(), scopes);

        using SpotifyClient client = new(credential);

        using MusicTrainner analizer = new(config);

        var track = await client.GetTrackAsync("1nDgPqq5gsQince3gJJ6dQ");

        await analizer.AddToTrainnigAsync(track, new short[] { 1, 3, 4 });
    }

    private static string GetCode()
    {
        var uri = new Uri(OAuthCredential.RedirectUri);

        if (!string.IsNullOrEmpty(uri.PathAndQuery))
            uri = new(uri.OriginalString.Replace(uri.PathAndQuery, "/"));

        HttpListener listner = new();
        listner.Prefixes.Add(uri.AbsoluteUri);
        listner.Start();

        var ctx = listner.GetContext();

        return ctx.Request.QueryString["code"] ?? string.Empty;
    }
}