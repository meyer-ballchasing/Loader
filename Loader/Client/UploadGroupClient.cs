using JetBrains.Annotations;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Client
{
    public class UploadGroupClient
    {
        public const string BaseEndpoint = "https://ballchasing.com/api";

        private readonly IRestClient restClient;
        private readonly IFileUploadClient fileClient;

        private readonly string accessKey;

        public UploadGroupClient([NotNull] IRestClient restClient, [NotNull] IFileUploadClient fileUploadClient, [NotNull] string accessKey)
        {
            this.restClient = restClient;
            this.fileClient = fileUploadClient;
            this.accessKey = accessKey;
        }

        public async Task PushGroupRecursive([NotNull] Group group)
        {
            await this.UpsertGroup(group);

            await this.UpsertReplays(group);

            await this.AssertGroupSize(group.BallChasingId, group.Replays.Count());

            foreach (var child in group.Children)
                await this.PushGroupRecursive(child);
        }

        private async Task UpsertGroup(Group group)
        {
            var body = new Dictionary<string, string>
            {
                { "name", group.Name },
                { "player_identification", "by-id" },
                { "team_identification", "by-distinct-players" }
            };

            if (group.Parent != null && !string.IsNullOrWhiteSpace(group.Parent.BallChasingId))
                body.Add("parent", group.Parent.BallChasingId);

            try
            {
                var groupResponse = await restClient.HttpPost<Dictionary<string, string>, Dictionary<string, string>>("groups", body, headers: this.GetHeaders());

                group.BallChasingId = groupResponse.Result["id"];
            }
            catch (HttpClientException e) when (e.HttpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                var query = new Dictionary<string, string>
                {
                    { "name", group.Name },
                    { "creator", "me" }
                };

                if (group.Parent != null && !string.IsNullOrWhiteSpace(group.Parent.BallChasingId))
                    query.Add("group", group.Parent.BallChasingId);

                var groupResponse = await restClient.HttpGet<JObject>("groups", query, headers: this.GetHeaders());

                group.BallChasingId = groupResponse.Result["list"][0]["id"].Value<string>();
            }
        }

        private async Task UpsertReplays(Group group)
        {
            foreach (var replay in group.Replays)
            {
                try
                {
                    var uploadResponse = await fileClient.Upload<Dictionary<string, string>>
                    (
                        "v2/upload",
                        replay.LocalFile,
                        parameters: new Dictionary<string, string>
                        {
                            { "visibility", replay.Public ? "public" : "private" }
                        },
                        headers: this.GetHeaders()
                    );

                    replay.BallChasingId = uploadResponse.Result["id"];

                    await PutReplayInGroup(replay);
                }
                catch (HttpClientException e) when (e.HttpResponseMessage.StatusCode == HttpStatusCode.Conflict)
                {
                    var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(await e.HttpResponseMessage.Content.ReadAsStringAsync());

                    replay.BallChasingId = body["id"].ToString();

                    await PutReplayInGroup(replay);
                }
            }
        }

        private async Task PutReplayInGroup(Replay replay)
        {
            await restClient.HttpPatch<dynamic, Dictionary<string, string>>($"replays/{replay.BallChasingId}", new
            {
                group = replay.Group.BallChasingId
            },
            headers: this.GetHeaders());
        }

        private async Task AssertGroupSize(string groupId, int count)
        {
            int replaysInGroup;
            do
            {
                replaysInGroup = 
                (
                    await restClient.HttpGet<ReplayListResponse>(
                        "replays",
                        parameters: new Dictionary<string, string>
                        {
                            { "group", groupId }
                        },
                        headers: this.GetHeaders()
                    )
                )
                .Result
                .Count;
            } while (replaysInGroup < count);
        }

        private Dictionary<string, string> GetHeaders() => new Dictionary<string, string> { { "Authorization", this.accessKey } };
    }
}