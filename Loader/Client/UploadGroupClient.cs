using JetBrains.Annotations;
using Meyer.BallChasing.Client.Models;
using Meyer.BallChasing.Models;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

            foreach (var child in group.Children)
                await this.PushGroupRecursive(child);
        }

        private async Task UpsertGroup(Group group)
        {
            if (group.BallChasingId != null)
                return;

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
            foreach (var replay in group.Replays.Where(x => string.IsNullOrEmpty(x.BallChasingId)))
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

            await this.AssertGroup(group);
        }

        private async Task PutReplayInGroup(Replay replay)
        {
            await restClient.HttpPatch<dynamic, Dictionary<string, string>>($"replays/{replay.BallChasingId}", new
            {
                group = replay.Group.BallChasingId
            },
            headers: this.GetHeaders());
        }

        private async Task<bool> AssertGroup(Group group)
        {
            var time = DateTime.Now;

            ReplayListResponse response = null;
            do
            {
                response =
                (
                    await restClient.HttpGet<ReplayListResponse>(
                        "replays",
                        parameters: new Dictionary<string, string>
                        {
                            { "group", group.BallChasingId },
                            { "count", "200" }
                        },
                        headers: this.GetHeaders()
                    )
                )
                .Result;
            } while (response.Count < group.Replays.Count && (DateTime.Now - time).TotalSeconds < 90);

            if(response.Count != group.Replays.Count)
            {
                var groupResponse = await restClient.HttpGet<JObject>($"groups/{group.BallChasingId}", headers: this.GetHeaders());

                if (groupResponse.Result["failed_replays"] != null)
                    return false;
            }

            return response.List
                .Select(x => x.Id)
                .SequenceEqual(group.Replays.Select(x => x.BallChasingId));
        }

        private Dictionary<string, string> GetHeaders() => new Dictionary<string, string> { { "Authorization", this.accessKey } };
    }
}