using Newtonsoft.Json;

namespace Nexus.Party.Master.Domain.Models.Spotify;

public class Track
{
    public string Id { get; set; }
    public int Popularity { get; set; }
    public bool Explicit { get; set; }
    [JsonProperty("is_local")]
    public bool IsLocal { get; set; }
    public string Name { get; set; }
    [JsonProperty("preview_url")]
    public string PreviewUrl { get; set; }
    public IEnumerable<Artist> Artists { get; set; }
}

public class Artist
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public int Popularity { get; set; }
    public IEnumerable<string> Genres { get; set; }
}