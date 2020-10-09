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

        public async Task OutputGameSummary(Group group)
        {
            foreach (var child in group.Children)
            {
                Spreadsheet spreadsheet = new Spreadsheet
                {
                    Sheets = new List<Sheet>(),
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{child.Name} Game Summary"
                    }
                };

                foreach (var childDepth2 in child.Children)
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Match" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Name" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "TeamName" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Win" } },
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Duration" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Overtime" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Id" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Platform" } }
                                        }
                                    }
                                }
                            }
                        },
                        Properties = new SheetProperties
                        {
                            Title = childDepth2.Name
                        }
                    });

                    foreach (var childDepth4 in childDepth2.Children.SelectMany(x => x.Children))
                    {
                        spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth4.Parent.Parent.Name).Data[0].RowData = spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth4.Parent.Parent.Name).Data[0].RowData.Union
                        (
                            childDepth4
                                .Replays
                                .SelectMany(x => ReplayPlayerSummary.GetSummary(x)).Select(x => new RowData
                                {
                                    Values = new List<CellData>
                                    {
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Replay.Group.Parent.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Replay.Group.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.TeamName } },
                                        new CellData { UserEnteredValue = new ExtendedValue { BoolValue = x.IsWin } },
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
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Duration } },
                                        new CellData { UserEnteredValue = new ExtendedValue { BoolValue = x.Overtime } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Id } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Platform } }
                                    }
                                })
                        )
                        .ToList();
                    }
                }

                await this.CreateSpreadsheetAsync(spreadsheet);
            }
        }

        public async Task OutputGroupSummary(Group group)
        {
            foreach (var child in group.Children)
            {
                Spreadsheet spreadsheet = new Spreadsheet
                {
                    Sheets = new List<Sheet>(),
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{child.Name} Group Summary"
                    }
                };

                foreach (var childDepth2 in child.Children)
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Match" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Name" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "TeamName" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "GP" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "GW" } },
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Duration" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Overtimes" } },
                                        }
                                    }
                                }
                            }
                        },
                        Properties = new SheetProperties
                        {
                            Title = childDepth2.Name
                        }
                    });

                    foreach (var childDepth4 in childDepth2.Children.SelectMany(x => x.Children))
                    {
                        spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth4.Parent.Parent.Name).Data[0].RowData = spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth4.Parent.Parent.Name).Data[0].RowData.Union
                        (
                            GroupPlayerSummary.GetSummary(childDepth4)
                                .Select(x => new RowData
                                {
                                    Values = new List<CellData>
                                    {
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Group.Parent.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Group.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Name } },
                                        new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.TeamName } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.GamesPlayed } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.GamesWon } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Mvp } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Score } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Goals } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Assists } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saves } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Shots } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Cycles } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saviors } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Inflicted } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Taken } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Duration } },
                                        new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Overtimes } }
                                    }
                                })
                        )
                        .ToList();
                    }
                }

                await this.CreateSpreadsheetAsync(spreadsheet);
            }
        }

        public async Task OutputSummaryAcrossGroups(Group group)
        {
            foreach (var child in group.Children)
            {
                Spreadsheet spreadsheet = new Spreadsheet
                {
                    Sheets = new List<Sheet>(),
                    Properties = new SpreadsheetProperties
                    {
                        Title = $"{child.Name} Summary"
                    }
                };

                foreach (var childDepth2 in child.Children)
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Name" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "GP" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "GW" } },
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
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Duration" } },
                                            new CellData { UserEnteredValue = new ExtendedValue { StringValue = "Overtimes" } }
                                        }
                                    }
                                }
                            }
                        },
                        Properties = new SheetProperties
                        {
                            Title = childDepth2.Name
                        }
                    });

                    spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth2.Name).Data[0].RowData = spreadsheet.Sheets.Single(x => x.Properties.Title == childDepth2.Name).Data[0].RowData.Union
                    (
                         GroupPlayerSummary.GetChildrenSummary(childDepth2)
                            .Select(x => new RowData
                            {
                                Values = new List<CellData>
                                {
                                    new CellData { UserEnteredValue = new ExtendedValue { StringValue = x.Name } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.GamesPlayed } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.GamesWon } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Mvp } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Score } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Goals } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Assists } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saves } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Shots } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Cycles } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Saviors } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Inflicted } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Taken } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Duration } },
                                    new CellData { UserEnteredValue = new ExtendedValue { NumberValue = x.Overtimes } }
                                }
                            })
                    )
                    .ToList();
                }

                await this.CreateSpreadsheetAsync(spreadsheet);
            }
        }

        private async Task CreateSpreadsheetAsync(Spreadsheet spreadsheet)
        {
            var spreadsheetResponse = await service.Spreadsheets.Create(spreadsheet).ExecuteAsync();

            var permissionRequest = driveService.Permissions.Create(new Permission
            {
                Type = "user",
                EmailAddress = googleCredentialInfo["userEmail"],
                Role = "owner",
            }, spreadsheetResponse.SpreadsheetId);

            permissionRequest.TransferOwnership = true;

            await permissionRequest.ExecuteAsync();
        }
    }
}