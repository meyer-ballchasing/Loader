using System;
using System.Diagnostics;

namespace Meyer.BallChasing.Loader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Aliases alias = Enum.Parse<Aliases>(args[0], true);
                switch (alias)
                {
                    case Aliases.push:
                        args[0] = "Meyer.BallChasing.Push.dll";
                        break;
                    case Aliases.pull:
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
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Please specify one of the actions: {string.Join(',', Enum.GetNames(typeof(Aliases)))} along with required arguments");
                Environment.Exit(-1);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Please specify one of the actions: {string.Join(',', Enum.GetNames(typeof(Aliases)))} along with required arguments");
                Environment.Exit(-1);
            }
        }
    }

    enum Aliases
    {
        push,
        pull
    }
}