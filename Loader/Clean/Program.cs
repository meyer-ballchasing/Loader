using Meyer.BallChasing.Models;
using Meyer.Common.Console;
using System;
using System.IO;
using System.Linq;

namespace Clean
{
    class Program
    {
        private static bool cleanShadow;
        private static bool cleanCsv;

        private static readonly ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new BooleanConsoleParameter(new[] { "shadow" }, () => cleanShadow, "Clean the shadow"),
                new BooleanConsoleParameter(new[] { "csv" }, () => cleanCsv, "Clean the output csv"),
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(cleanShadow && x.EnumerateFiles(Constants.SavedStateFileName).Any())
                        File.Delete($"{x.FullName}/{Constants.SavedStateFileName}");

                    if(cleanCsv)
                    {
                        foreach (var item in x.EnumerateFiles("*.csv", SearchOption.AllDirectories))
                            item.Delete();
                    }
                }, "The path to the root directory containing the replay files", true),
            }
        };

        static void Main(string[] args)
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
        }
    }
}