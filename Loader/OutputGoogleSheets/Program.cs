using Meyer.BallChasing.Models;
using Meyer.Common.Console;
using System;
using System.IO;
using System.Linq;

namespace OutputGoogleSheets
{
    class Program
    {
        private static DirectoryInfo rootDirectory;

        private static ConsoleParameters consoleParameters = new ConsoleParameters
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
                }, "The path to the root directory containing the replay files to push", true)
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