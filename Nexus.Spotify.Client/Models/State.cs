namespace Nexus.Spotify.Client.Models;

public class State : Model
{
    [JsonProperty("is_playing")]
    public bool IsPlaying { get; set; }

    public long TimeStamp { get; set; }
    
    [JsonProperty("progress_ms")]
    public int ProgressMilisseconds { get; set; }

    [JsonProperty("item")]
    public Track Item { get; set; }
}