using Meyer.BallChasing.Client;
using Meyer.BallChasing.Models;
using Meyer.BallChasing.PullStats.OutputStrategies;
using Meyer.Common.Console;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats
{
    class Program
    {
        private static DirectoryInfo rootDirectory;
        
        private static ParsedReplayClient ballChasingClient;
        
        private static Outputs output = Outputs.csv;
        
        private static bool outputReplaySummary;
        private static bool outputGroupSummary;
        private static bool outputAcrossGroupSummary;

        private static readonly ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(!x.EnumerateFiles(Constants.SavedStateFileName).Any())
                        throw new Exception(x.FullName);

                    rootDirectory = x;
                }, "The path to the root directory containing the replay files", true),
                new ActionConsoleParameter(new[] { "key" }, x =>
                {
                    ballChasingClient = new ParsedReplayClient
                    (
                        new RestClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions
                        {
                            RetryPolicy = new BackOffRetryPolicy()
                        }), x
                    );
                }, "The access key for ballchasing.com/api", true),
                new NamedEnumConsoleParameter<Outputs>(new[] { "o" }, () => output, $"The output strategy ({string.Join(", ", Enum.GetNames(typeof(Outputs)).Skip(1))}) to use. Default: csv"),
                new BooleanConsoleParameter(new[] { "replaysummary" }, () => outputReplaySummary, "Output summaries for each replay"),
                new BooleanConsoleParameter(new[] { "groupsummary" }, () => outputGroupSummary, "Output summaries for group"),
                new BooleanConsoleParameter(new[] { "summary" }, () => outputAcrossGroupSummary, "Output summary across all groups"),
            }
        };

        static async Task Main(string[] args)
        {
            try
            {
                consoleParameters.Map(args, false);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Console.WriteLine(consoleParameters.ToString());

                Environment.Exit(-1);
            }

            Group shadow = JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{Constants.SavedStateFileName}"));

            await ballChasingClient.PullParsedReplays(shadow);

            IOutputStrategy outputStrategy = OutputStrategyFactroy.GetOutputStrategyAsync(output, rootDirectory);

            await outputStrategy.OutputGameSummary(shadow);
            await outputStrategy.OutputGroupSummary(shadow);
            await outputStrategy.OutputSummaryAcrossGroups(shadow);
        }
    }
}