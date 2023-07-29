﻿using Microsoft.Extensions.Configuration;
using Nexus.Party.Master.Categorizer.Analizer;
using Nexus.Party.Master.Categorizer.Models;
using Nexus.Spotify.Client;
using System.Diagnostics;
using System.Net;

public class Program
{
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.Development.json"), true)
            .Build();

        using SpotifyClient client = await Utils.GetConsoleClientAsync(config);
        using MusicTrainner analizer = new();

        var track = await client.GetTrackAsync("1nDgPqq5gsQince3gJJ6dQ");

        await analizer.AddToTrainnigAsync(track, new string[] { "funk" });
    }
}