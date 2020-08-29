using Meyer.BallChasing.Client;
using Meyer.Common.Console;
using Meyer.Common.HttpClient;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Push
{
    class Program
    {
        private static DirectoryInfo inputDirectory;
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

                    inputDirectory = x;
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

        static async Task Main(string[] args)
        {
            consoleParameters.Map(args, false);

            await ballChasingClient.PushGroupRecursive(new Group(inputDirectory, null));
        }
    }
}