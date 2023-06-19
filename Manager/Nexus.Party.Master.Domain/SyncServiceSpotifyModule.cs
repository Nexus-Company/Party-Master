using Newtonsoft.Json;
using Nexus.Party.Master.Domain.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.Party.Master.Domain
{
    public partial class SyncService
    {
        const string PlayerUrl = "https://api.spotify.com/v1/me/player";
        public IEnumerable<Track> Queue { get; set; }
     
        private protected async Task<State> GetStatus(CancellationToken stoppingToken)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(PlayerUrl)
            };
            request.Headers.Authorization = Credential!.GetHeader();

            var response = await Client.SendAsync(request, stoppingToken);

            string state = await response.Content.ReadAsStringAsync(stoppingToken);

            return JsonConvert.DeserializeObject<State>(state)!;
        }

        private protected async Task DefineQueueAsync(CancellationToken stoppingToken)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{PlayerUrl}/queue")
            };
            request.Headers.Authorization = Credential!.GetHeader();

            var response = await Client.SendAsync(request, stoppingToken);

            string state = await response.Content.ReadAsStringAsync(stoppingToken);

            var rst = JsonConvert.DeserializeObject<QueueResult>(state)!;

            Queue = rst.Queue;
        }

        private class QueueResult {

            public IEnumerable<Track> Queue { get; set; }
        }
    }
}
