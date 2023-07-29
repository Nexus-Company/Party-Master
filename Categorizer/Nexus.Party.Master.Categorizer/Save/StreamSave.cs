using Nexus.Party.Master.Categorizer.Models;
using System.Text;

namespace Nexus.Party.Master.Categorizer.Save;

internal class StreamSave : IDisposable
{
    private readonly Stream output;
    private GenreConvert genreConvert;
    private readonly IDictionary<string, Trainning> results;
    public StreamSave(Stream stream, GenreConvert genres, IDictionary<string, Trainning> musics)
    {
        output = stream;
        genreConvert = genres;
        results = musics;
    }

    public async Task SaveToStreamAsync()
    {
        await SaveHeaderAsync();

        await SaveToGenreAsync();

        foreach (var item in results)
            await SaveTrainnigAsync(item.Value);
    }

    private async Task SaveHeaderAsync()
    {
        byte[] buffer = BitConverter.GetBytes(genreConvert.keys.Count);

        await output.WriteAsync(buffer);

        buffer = BitConverter.GetBytes(results.Count);

        await output.WriteAsync(buffer);

        await output.FlushAsync();
    }

    private async Task SaveToGenreAsync()
    {
        byte[] buffer;
        byte[] aux;
        foreach (var item in genreConvert.keys)
        {
            buffer = Encoding.ASCII.GetBytes(item.Key);
            aux = BitConverter.GetBytes(buffer.Length);

            await output.WriteAsync(aux);
            await output.WriteAsync(buffer);
        }

        await output.FlushAsync();
    }

    private async Task SaveTrainnigAsync(Trainning trainning)
    {
        await output.WriteAsync(
            BitConverter.GetBytes(trainning.Genres.Length));

        await output.WriteAsync(
            BitConverter.GetBytes(trainning.Mfccs.Length));

        foreach (var item in trainning.Genres)
            await output.WriteAsync(
                BitConverter.GetBytes(item));

        foreach (var item in trainning.Mfccs)
            await output.WriteAsync(
                BitConverter.GetBytes(item));

        await output.FlushAsync();
    }

    public void Dispose()
    {
        output.Dispose();

        GC.Collect();
    }
}