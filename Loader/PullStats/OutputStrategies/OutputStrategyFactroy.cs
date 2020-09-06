using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Meyer.BallChasing.PullStats.OutputStrategies
{
    public static class OutputStrategyFactroy
    {
        public static IOutputStrategy GetOutputStrategyAsync(Outputs output, DirectoryInfo rootDirectory)
        {
            switch (output)
            {
                case Outputs.sheets:
                    var googleCredentialInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText($"{rootDirectory.FullName}/google.json"));

                    var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(googleCredentialInfo["serviceAccountEmail"])
                    {
                        Scopes = new[] { SheetsService.Scope.Spreadsheets, DriveService.Scope.Drive }
                    }
                    .FromCertificate(new X509Certificate2($"{rootDirectory.FullName}/google.p12", googleCredentialInfo["privateKeyPassword"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable)));

                    return new GoogleSheetOutputStrategy(new SheetsService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.Exception
                    }),
                    new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential
                    }),
                    googleCredentialInfo);
                case Outputs.csv:
                default:
                    return new CsvOutputStrategy(rootDirectory);
            }
        }
    }
}