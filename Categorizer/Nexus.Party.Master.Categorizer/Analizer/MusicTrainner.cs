using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Nexus.Party.Master.Categorizer.Save;
using Nexus.Party.Master.Categorizer.Models;
using Nexus.Spotify.Client.Models;
using System.Data;
using Accord.Math;

namespace Nexus.Party.Master.Categorizer.Analizer;

public class MusicTrainner : MusicAnalizerBase
{
    readonly Dictionary<string, Trainning> results;
   
    MultilabelSupportVectorMachine<Gaussian>? machine;
    public MusicTrainner() : base(new GenreConvert())
    {
        results = new Dictionary<string, Trainning>();
    }

    public async Task AddToTrainnigAsync(Track track, string[] genres)
    {
        var stream = await DownloadAsync(track);

        var mfccs = CalculateMFCCs(stream);

        results.Add(track.Id, new Trainning(ConvertGenres(genres), mfccs));
    }

    public void Proccess()
    {
        var data = new List<MusicData>();

        foreach (var track in results)
            data.Add(new(track.Value, genreConvert));

        dataset = data.ToArray();
    }

    public void Trainnig()
    {
        if (dataset is null)
            throw new ArgumentException("Os dados de entrada devem ser processados anteriormente.");

        var trainingInputs = dataset.Select(unit => unit.Mfccs).ToArray();
        var trainingOutputs = dataset.Select(unit => unit.GenreLabel).ToArray();

        // Treinamento do modelo SVM multirrótulo com o dataset de treinamento
        machine = TrainSVMModel(trainingInputs, trainingOutputs);
    }

    public async Task SaveToFileAsync(string fileName)
        => await SaveToStreamAsync(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write));

    public async Task SaveToStreamAsync(Stream stream)
    {
        using var fileSave = new StreamSave(stream, genreConvert, results);

        await fileSave.SaveToStreamAsync();
    }

    #region Auxiliary
    private short[] ConvertGenres(string[] genres)
    {
        short[] genresShort = new short[genres.Length];
        for (int i = 0; i < genres.Length; i++)
        {
            string item = genres[i];
            short genre;

            try
            {
                genre = genreConvert.GetGenre(item);
            }
            catch (Exception)
            {
                genre = genreConvert.AddGenre(item);
            }

            genresShort[i] = genre;
        }
        return genresShort;
    }

    // TODO: Criar módulo que evolui o modelo atual 
    #endregion
}
