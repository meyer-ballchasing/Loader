using Meyer.Common.Console;
using System;
using System.Diagnostics;

namespace Meyer.BallChasing.Loader
{
    class Program
    {
        private static Aliases alias;

        private static ConsoleParameters consoleParameters = new ConsoleParameters
        {
            OrderedConsoleParameters =
            {
                new OrderedEnumConsoleParameter<Aliases>(() => alias, "Specify one of the actions")
            }
        };

        static void Main(string[] args)
        {
            try
            {
                consoleParameters.Map(args, true);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Console.WriteLine(consoleParameters.ToString());

                Environment.Exit(-1);
            }

            switch (alias)
            {
                case Aliases.push:
                    args[0] = "Meyer.BallChasing.Push.dll";
                    break;
                case Aliases.stats:
                    args[0] = "Meyer.BallChasing.PullStats.dll";
                    break;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = string.Join(" ", args),
                WorkingDirectory = Environment.CurrentDirectory
            })
            .WaitForExit();
        }
    }

    enum Aliases
    {
        @null,
        push,
        stats
    }
}