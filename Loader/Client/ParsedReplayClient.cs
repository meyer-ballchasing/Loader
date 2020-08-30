using JetBrains.Annotations;
using Meyer.BallChasing.Models;
using Meyer.Common.HttpClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Client
{
    public class ParsedReplayClient
    {
        public const string BaseEndpoint = "https://ballchasing.com/api";

        private readonly IRestClient restClient;

        private readonly string accessKey;

        public ParsedReplayClient([NotNull] IRestClient restClient, [NotNull] string accessKey)
        {
            this.restClient = restClient;
            this.accessKey = accessKey;
        }

        public async Task PullParsedReplays(Group group)
        {
            foreach (var child in group.Children)
                await this.PullParsedReplays(child);

            foreach (var replay in group.Replays)
            {
                var replayResponse = await restClient.HttpGet<ProcessedReplay>
                (
                    $"replays/{replay.BallChasingId}",
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", this.accessKey }
                    }
                );

                replay.ProcessedReplay = replayResponse.Result;
            }
        }
    }
}