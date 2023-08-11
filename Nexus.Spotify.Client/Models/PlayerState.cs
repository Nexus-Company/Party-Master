namespace Nexus.Spotify.Client.Models;

public class PlayerState : Model
{
    [JsonProperty("is_playing")]
    public bool IsPlaying { get; set; }

    public long TimeStamp { get; set; }

    [JsonProperty("progress_ms")]
    public int ProgressMilisseconds { get; set; }
    public Track Item { get; set; }
    public Device Device { get; set; }

    public async Task SkipAsync()
    {
        var request = client.CreateRequest($"{SpotifyClient.PlayerUrl}/next");
        request.Method = HttpMethod.Post;

        await client.HttpClient.SendAsync(request);
    }
}

public class Device
{
    public string? Id { get; set; }
    [JsonProperty("is_active")]
    public bool Active { get; set; }
    public string Name { get; set; }
    [JsonProperty("volume_percent")]
    public int? Volume { get; set; }
}