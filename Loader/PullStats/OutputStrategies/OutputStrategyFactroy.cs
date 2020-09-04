using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats.OutputStrategies
{
    public static class OutputStrategyFactroy
    {
        public static async Task<IOutputStrategy> GetOutputStrategyAsync(Outputs output, DirectoryInfo rootDirectory)
        {
            switch (output)
            {
                case Outputs.sheets:
                    UserCredential credential;

                    using (var stream = new FileStream($"{rootDirectory.FullName}/credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        credential = await GoogleWebAuthorizationBroker.AuthorizeAsync
                        (
                            GoogleClientSecrets.Load(stream).Secrets,
                            new[] { SheetsService.Scope.Spreadsheets },
                            "user",
                            CancellationToken.None,
                            new FileDataStore("token.json", false)
                        );
                    }

                    return new GoogleSheetOutputStrategy(new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential
                    }));
                case Outputs.csv:
                default:
                    return new CsvOutputStrategy();
            }

            
        }
    }
}
