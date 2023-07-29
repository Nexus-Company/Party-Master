namespace Nexus.Party.Master.Categorizer.Models;

internal class MusicData
{
    public double[] Mfccs { get; set; }
    public int GenreLabel { get; set; }

    public MusicData(Trainning trainning)
    {
        Mfccs = trainning.Mfccs;
        GenreLabel = CombineShortListToInt(trainning.Genres);
    }

    private static short[] SplitIntToShortList(int combinedValue)
    {
        List<short> shorts = new();

        for (int i = 0; i < sizeof(int) * 8; i++)
        {
            if ((combinedValue & (1 << i)) != 0)
            {
                shorts.Add((short)i);
            }
        }

        return shorts.ToArray();
    }

    private static int CombineShortListToInt(IEnumerable<short> shorts)
    {
        int combinedValue = 0;

        foreach (short value in shorts)
        {
            combinedValue |= 1 << value;
        }

        return combinedValue;
    }
}
