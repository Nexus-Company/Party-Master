using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Nexus.Party.Master.Categorizer.Save;
using Nexus.Party.Master.Categorizer.Models;
using Nexus.Spotify.Client.Models;
using System.Data;
using Accord.Math;
using System.Collections.Concurrent;

namespace Nexus.Party.Master.Categorizer.Analizer;

public class MusicTrainner : MusicAnalizerBase
{
    readonly ConcurrentDictionary<string, Trainning> results;
    readonly List<Task> tasks;

    MultilabelSupportVectorMachine<Gaussian>? machine;
    public MusicTrainner() : base(new GenreConvert())
    {
        results = new();
        tasks = new();
    }
    int mfccsCount = 0;
    public void AddToTrainning(Track track, string[] genres)
    {
        async void Add(object? obj)
        {
            try
            {
                var trainning = (AddTrainning)obj!;
                bool exist = results.TryGetValue(trainning.Track.Id, out _);

                if (exist)
                {
                    var track = results[trainning.Track.Id];
                    var genres = ConvertGenres(trainning.Genres);

                    List<short> genreRst = new();
                    genreRst.AddRange(track.Genres);

                    foreach (short item in genres)
                    {
                        if (!genreRst.Contains(item))
                            genreRst.Add(item);
                    }

                    results[trainning.Track.Id] = new Trainning(genreRst.ToArray(), track.Mfccs);

                    Console.WriteLine($"Music id \"0{trainning.Track.Id}\" rewrite genres.");
                }

                var stream = await DownloadAsync(trainning.Track);

                var mfccs = CalculateMFCCs(stream);

                if (mfccsCount == 0)
                    mfccsCount = mfccs.Length;

                if (mfccsCount != mfccs.Length)
                    throw new ArgumentException("Track Mfccs length wrong");

                results.TryAdd(trainning.Track.Id, new Trainning(ConvertGenres(trainning.Genres), mfccs));
            }
            catch (Exception ex)
            {

            }
        }

        tasks.Add(new Task(Add, new AddTrainning(track, genres)));
    }

    public async Task ProccessAsync()
    {
        foreach (var task in tasks)
            if (task.Status == TaskStatus.Created)
                task.Start();

        await Task.WhenAll(tasks);

        // Realize a coleta de lixo após a conclusão das tarefas
        GC.Collect();
        GC.WaitForPendingFinalizers();

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
                genre = genreConvert.Get(item);
            }
            catch (Exception)
            {
                genre = genreConvert.Add(item);
            }

            genresShort[i] = genre;
        }
        return genresShort;
    }

    // TODO: Criar módulo que evolui o modelo atual 
    #endregion


    private struct AddTrainning
    {
        public AddTrainning(Track track, string[] genres)
        {
            Track = track ?? throw new ArgumentNullException(nameof(track));
            Genres = genres ?? throw new ArgumentNullException(nameof(genres));
        }

        public Track Track { get; set; }
        public string[] Genres { get; set; }
    }
}
