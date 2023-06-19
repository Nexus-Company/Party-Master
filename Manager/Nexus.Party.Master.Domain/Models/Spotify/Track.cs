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
    /// <summary>
    /// Spotify 30 seconds audio preview.
    /// </summary>
    [JsonProperty("preview_url")]
    public string? PreviewUrl { get; set; }
    public string Uri { get; set; }
    public IEnumerable<Artist> Artists { get; set; }
    [JsonProperty("external_urls")]
    public IDictionary<string,string> Urls { get; set; }
    public Album Album { get; set; }
}


public class Album {
    public IEnumerable<Image>? Images { get; set; }
    public string Name { get; set; }
}