using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Api.Models;

public class TrackActual
{
    public bool IsPlaying { get; set; }
    public long TimeStamp { get; set; }
    public int ProgressMilisseconds { get; set; }
    public Track Track { get; set; }

    public TrackActual(State state)
    {
        IsPlaying = state.IsPlaying;
        TimeStamp = state.TimeStamp;
        Track = state.Item;
        ProgressMilisseconds = state.ProgressMilisseconds;
    }
}