using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Categorizer;

public class MusicAnalizer : IDisposable
{
    public Guid Id { get; private set; }
    public Track Track { get; private set; }
    private string TempFile { get; set; }
    private static string? TempPath;
    public MusicAnalizer(Track track)
    {
        Track = track;
        Id = Guid.NewGuid();
        TempPath ??= Path.Combine(Path.GetTempPath(), "Music-Analizer");
        TempFile = @$"{TempPath}\{Id}.mp3";
    }

    public async Task DownloadAsync()
    {
        using FileStream stream = new(TempFile, FileMode.OpenOrCreate, FileAccess.Write);

        await Track.DownloadPreviewAsync(stream);
    }

    public void Dispose()
    {
        if (File.Exists(TempFile))
            File.Delete(TempFile);
    }
}