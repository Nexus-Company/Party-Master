using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using Microsoft.Extensions.Configuration;
using Nexus.Spotify.Client.Models;

namespace Nexus.Party.Master.Categorizer.Analizer;

public class MusicAnalizer : MusicAnalizerBase
{
    MulticlassSupportVectorMachine<SupportVectorMachine<IKernel<float[]>, float[]>, IKernel<float[]>, float[]> machine;
    public MusicAnalizer(IConfiguration config) : base(new Models.GenreConvert())
    {

    }

    public async Task<string> GetGenrAsync(Track track)
    {
        var str = await DownloadAsync(track);

        var mfccs = CalculateMFCCs(str);

        throw new NotImplementedException();
    }
}