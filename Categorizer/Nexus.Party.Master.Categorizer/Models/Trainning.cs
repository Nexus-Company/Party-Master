namespace Nexus.Party.Master.Categorizer.Models;

internal struct Trainning
{
    public short[] Genres { get; set; }
    public double[] Mfccs { get; set; }

    public Trainning(short[] genres, double[] mfccs)
    {
        Genres = genres;
        Mfccs = mfccs;
    }
}