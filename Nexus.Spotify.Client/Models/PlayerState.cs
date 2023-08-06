namespace Nexus.Spotify.Client.Models;

public class PlayerState : Model
{
    [JsonProperty("is_playing")]
    public bool IsPlaying { get; set; }

    public long TimeStamp { get; set; }

    [JsonProperty("progress_ms")]
    public int ProgressMilisseconds { get; set; }

    [JsonProperty("item")]
    public Track Item { get; set; }


    public async Task SkipAsync()
    {
        var request = client.CreateRequest($"{SpotifyClient.PlayerUrl}/next");
        request.Method = HttpMethod.Post;

        await client.HttpClient.SendAsync(request);
    }
}