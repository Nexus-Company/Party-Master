using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Nexus.Spotify.Client.Models;
using NWaves.FeatureExtractors;
using NWaves.FeatureExtractors.Options;

namespace Nexus.Party.Master.Categorizer.Analizer;

public abstract class MusicAnalizerBase : IDisposable
{
    public Guid Id { get; private set; }
    private readonly string TempPath;
    private readonly IConfiguration Config;

    public MusicAnalizerBase(IConfiguration config)
    {
        Id = Guid.NewGuid();
        TempPath ??= Path.Combine(Path.GetTempPath(), "Music-Analizer");
        Config = config;

        if (!Directory.Exists(TempPath))
            Directory.CreateDirectory(TempPath);

        TempPath = Path.Combine(TempPath, Id.ToString());

        if (!Directory.Exists(TempPath))
            Directory.CreateDirectory(TempPath);
    }

    #region Auxiliary
    private protected async Task<Stream> DownloadAsync(Track track)
    {
        string tempFile = Path.Combine(TempPath, $"{track.Id}.mp3");

        var stream = new FileStream(tempFile, FileMode.CreateNew, FileAccess.ReadWrite);
        await track.DownloadPreviewAsync(stream);

        stream.Position = 0;

        return stream;
    }

    private protected static double[] CalculateMFCCs(Stream stream)
    {
        var samplesList = new List<float>();

        using (stream)
        {
            var mp3Reader = new Mp3FileReader(stream);

            var pmc = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
            var buffer = new float[1024];

            var provider = pmc.ToSampleProvider();

            while (provider.Read(buffer, 0, buffer.Length) > 0)
                samplesList.AddRange(buffer);
        }

        // Configurar o extrator de MFCCs
        var mfcc = new MfccExtractor(new MfccOptions
        {
            SamplingRate = 44100,
            FeatureCount = 13,
            FrameDuration = 0.0256,
            HopDuration = 0.010,
            FilterBankSize = 26,
            LowFrequency = 20,
            HighFrequency = 20000
        });

        float[] samples = samplesList.ToArray();

        var mfccs = mfcc.ComputeFrom(samples, 0, samples.Length);

        return ConvertMfccsToDouble(mfccs.ToArray());
    }

    private protected static MultilabelSupportVectorMachine<Gaussian> TrainSVMModel(double[][] inputs, int[] outputs)
    {
        // Treinar o modelo SVM multirrótulo com o dataset de treinamento
        var teacher = new MultilabelSupportVectorLearning<Gaussian>()
        {
            Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
            {
                Complexity = 100 // Valor do parâmetro de complexidade do SVM
            }
        };

        MultilabelSupportVectorMachine<Gaussian> machine = teacher.Learn(inputs, outputs);

        return machine;
    }

    // Método para converter os coeficientes MFCCs para double[]
    private static double[] ConvertMfccsToDouble(float[][] mfccs)
    {
        // Aqui você pode converter os coeficientes MFCCs de float[][] para double[]
        double[] convertedMfccs = new double[mfccs.Length * mfccs[0].Length];
        int index = 0;
        for (int i = 0; i < mfccs.Length; i++)
        {
            for (int j = 0; j < mfccs[i].Length; j++)
            {
                convertedMfccs[index] = mfccs[i][j];
                index++;
            }
        }
        return convertedMfccs;
    }

    #endregion

    public void Dispose()
    {
        if (Directory.Exists(TempPath))
            Directory.Delete(TempPath, true);
    }
}

public class MusicAnalizer : MusicAnalizerBase
{
    MulticlassSupportVectorMachine<SupportVectorMachine<IKernel<float[]>, float[]>, IKernel<float[]>, float[]> machine;
    public MusicAnalizer(IConfiguration config) : base(config)
    {

    }

    public async Task<string> GetGenrAsync(Track track)
    {
        var str = await DownloadAsync(track);

        var mfccs = CalculateMFCCs(str);

        throw new NotImplementedException();
    }
}