namespace Nexus.Party.Master.Categorizer.Models;

internal class MusicData
{
    public double[] Mfccs { get; set; }
    public bool[] GenreLabel { get; set; }

    public MusicData(Trainning trainning, GenreConvert genreConvert)
    {
        Mfccs = trainning.Mfccs;
        GenreLabel = new bool[genreConvert.keys.Count];

        for (int i = 0; i < GenreLabel.Length; i++)
        {
            short actual = genreConvert.keys.ElementAt(i).Value;
            
            if (trainning.Genres.Contains(actual))
            {
                GenreLabel[i] = true;
            }
            else
            {
                GenreLabel[i] = false;
            }
        }
    }
}
