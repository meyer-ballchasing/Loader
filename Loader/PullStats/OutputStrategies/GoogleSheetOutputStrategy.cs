using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Meyer.BallChasing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats.OutputStrategies
{
    public class GoogleSheetOutputStrategy : IOutputStrategy
    {
        private readonly SheetsService service;
        private readonly DriveService driveService;
        private readonly Dictionary<string, string> googleCredentialInfo;

        public GoogleSheetOutputStrategy(SheetsService service, DriveService driveService, Dictionary<string, string> googleCredentialInfo)
        {
            this.service = service;
            this.driveService = driveService;
            this.googleCredentialInfo = googleCredentialInfo;
        }

        public async Task Output(Group group)
        {
            Spreadsheet spreadsheet = null;

            this.OutputGameSummary(group, ref spreadsheet);

            var spreadsheetResponse = service.Spreadsheets.Create(spreadsheet).Execute();

            var permissionRequest = driveService.Permissions.Create(new Permission
            {
                Type = "user",
                EmailAddress = googleCredentialInfo["userEmail"],
                Role = "owner",
            }, spreadsheetResponse.SpreadsheetId);

            permissionRequest.TransferOwnership = true;

            permissionRequest.Execute();
        }

        private void OutputGameSummary(Group group, ref Spreadsheet spreadsheet, int depth = 0)
        {
            if (depth == 1)
            {
                spreadsheet = new Spreadsheet
                {
                    Sheets = new List<Sheet>(),
                    Properties = new SpreadsheetProperties
                    {
                        Title = group.Name
                    }
                };
            }

            if (depth == 2)
            {
                spreadsheet.Sheets.Add(new Sheet
                {
                    Data = new List<GridData>
                    {
                        new GridData
                        {
                            RowData = new List<RowData>
                            {
                                new RowData
                                {
                                    Values = new List<CellData>
                                    {
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Week" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "TeamName" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Name" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "TeamName" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Mvp" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Score" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Goals" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Assists" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Saves" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Shots" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Cycles" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Saviors" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Inflicted" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Taken" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Id" } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Platform" } }
                                    }
                                }
                            }
                        }
                    },
                    Properties = new SheetProperties
                    {
                        Title = group.Name
                    }
                });
            }

            if (depth == 4)
            {
                spreadsheet.Sheets.Single(x => x.Properties.Title == group.Parent.Parent.Name).Data[0].RowData = spreadsheet.Sheets.Single(x => x.Properties.Title == group.Parent.Parent.Name).Data[0].RowData.Union(
                    group
                        .Replays
                        .SelectMany(x => ReplayPlayerSummary.GetSummary(x)).Select(x => new RowData
                        {
                            Values = new List<CellData>
                            {
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Replay.Group.Parent.Name } },
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Replay.Group.Name } },
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Name } },
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.TeamName } },
                                new CellData { UserEnteredValue = new ExtendedValue { BoolValue = x.Mvp } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Score } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Goals } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Assists } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saves } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Shots } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Cycles } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saviors } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Inflicted } },
                                new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Taken } },
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Id } },
                                new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Platform } }
                            }
                        })).ToList();
            }

            if (depth <= 4)
            {
                foreach (var child in group.Children)
                    this.OutputGameSummary(child, ref spreadsheet, depth + 1);
            }
        }
    }
}