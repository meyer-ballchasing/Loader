using Meyer.BallChasing.Models;
using Meyer.Common.Console;
using System;
using System.IO;
using System.Linq;

namespace Clean
{
    class Program
    {
        private static readonly ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(x.EnumerateFiles(Constants.SavedStateFileName).Any())
                        File.Delete($"{x.FullName}/{Constants.SavedStateFileName}");

                    foreach (var item in x.EnumerateFiles("*.csv", SearchOption.AllDirectories))
                        item.Delete();

                }, "The path to the root directory containing the replay files", false)
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