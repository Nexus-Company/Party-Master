using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Nexus.Party.Master.Categorizer.Save; 
using Nexus.Party.Master.Categorizer.Models;
using Nexus.Spotify.Client.Models;
using System.Data;

namespace Nexus.Party.Master.Categorizer.Analizer;

public class MusicTrainner : MusicAnalizerBase
{
    readonly Dictionary<string, Trainning> results;
    readonly IEnumerable<MusicData>? dataset;
    readonly MulticlassSupportVectorLearning<Gaussian> teacher;
    public MusicTrainner() : base(new GenreConvert())
    {
        results = new Dictionary<string, Trainning>();
        teacher = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
            {
                Complexity = 100 // Valor do parâmetro de complexidade do SVM
            }
        };
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
            data.Add(new(track.Value));
    }

    public void Trainnig()
    {
        if (dataset is null)
            throw new ArgumentException("Os dados de entrada devem ser processados anteriormente.");

        int splitIndex = (int)(dataset.Count() * 0.8); // 80% para treinamento, 20% para teste
        var trainingData = dataset.Take(splitIndex).ToList();
        var testData = dataset.Skip(splitIndex).ToList();

        // Converter os coeficientes MFCCs para double[] antes de treinar o modelo SVM
        var trainingInputs = trainingData.Select(unit => unit.Mfccs).ToArray();
        var trainingOutputs = trainingData.Select(unit => unit.GenreLabel).ToArray(); // Obter as classes multirrótulo

        // Treinamento do modelo SVM multirrótulo com o dataset de treinamento
        var machine = TrainSVMModel(trainingInputs, trainingOutputs);

        // Avaliar o modelo usando o conjunto de teste
        var testInputs = testData.Select(unit => unit.Mfccs).ToArray();
        var testOutputs = testData.Select(unit => unit.GenreLabel).ToArray(); // Obter as classes multirrótulo para o conjunto de teste
        EvaluateModel(machine, testInputs, testOutputs);
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

    // Método para avaliar o modelo usando o conjunto de teste
    private static void EvaluateModel(MultilabelSupportVectorMachine<Gaussian> machine, double[][] testInputs, int[] testOutputs)
    {
        //int correctPredictions = 0;
        //int totalPredictions = testInputs.Length;

        //for (int i = 0; i < testInputs.Length; i++)
        //{
        //    int predictedLabels = machine.Decide(testOutputs);
        //    if (predictedLabels.SequenceEqual(testOutputs[i]))
        //    {
        //        correctPredictions++;
        //    }
        //}

        //double accuracy = (double)correctPredictions / totalPredictions;
        //Console.WriteLine("Acurácia do modelo: " + (accuracy * 100) + "%");
    }
    #endregion
}
