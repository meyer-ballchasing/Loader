using Meyer.BallChasing.Client;
using Meyer.Common.Console;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Push
{
    class Program
    {
        private static DirectoryInfo rootDirectory;
        private static UploadGroupClient ballChasingClient;

        private static ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(!x.EnumerateFiles("*.replay", SearchOption.AllDirectories).Any())
                        throw new Exception(x.FullName);

                    rootDirectory = x;
                }, "The path to the root directory containing the replay files to push", true),
                new ActionConsoleParameter(new[] { "key" }, x =>
                {
                    ballChasingClient = new UploadGroupClient
                    (
                        new RestClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions
                        {
                            RetryPolicy = new BackOffRetryPolicy()
                        }),
                        new FileUploadClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions()), x
                    );
                }, "The path to the root directory containing the replay files to push", true)
            }
        };

        private const string SavedStateFileName = "ballchasing.json";

        static async Task Main(string[] args)
        {
            consoleParameters.Map(args, false);

            Group newGroup = new Group(rootDirectory, null);
            Group savedState = null;

            if (rootDirectory.EnumerateFiles(SavedStateFileName).Any())
                savedState = JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{SavedStateFileName}"));

            await ballChasingClient.PushGroupRecursive(newGroup, savedState);

            await File.WriteAllTextAsync($"{rootDirectory.FullName}/{SavedStateFileName}", JsonConvert.SerializeObject(newGroup, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            }));
        }
    }
}