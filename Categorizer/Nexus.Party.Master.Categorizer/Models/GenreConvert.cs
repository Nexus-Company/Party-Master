namespace Nexus.Party.Master.Categorizer.Models;

internal struct GenreConvert
{
    public IDictionary<string, short> keys;

    public GenreConvert() : this(new Dictionary<string, short>())
    {
    }

    public GenreConvert(IDictionary<string, short> keys)
    {
        this.keys = new Dictionary<string, short>(keys.OrderBy(x => x.Value));
    }

    public readonly short AddGenre(string genre)
    {
        var last = keys.LastOrDefault();

        if (last.Equals(default(KeyValuePair<string, short>)))
            last = new(string.Empty, -1);

        short value = (short)(last.Value + 1);
        keys.Add(genre, value);
        return value;
    }

    public readonly short GetGenre(string genre)
        => keys[genre];
}