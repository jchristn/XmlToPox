namespace Test.Automated
{
    using System;
    using System.Threading.Tasks;
    using Test.Shared;
    using Touchstone.Cli;

    /// <summary>
    /// Console host for the shared XmlToPox Touchstone suites.  Runs every case
    /// defined in <see cref="XmlToPoxTestSuites.All"/> and returns a non-zero exit
    /// code when any case fails, so the runner is CI-friendly.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">
        /// Optional arguments.  Supported:
        ///   --results &lt;path&gt;   Write a JSON result file to the supplied path.
        ///   -h | --help          Print usage and exit.
        /// </param>
        /// <returns>Process exit code (0 on success, non-zero on any failure).</returns>
        public static async Task<int> Main(string[] args)
        {
            string? resultsPath = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        PrintUsage();
                        return 0;

                    case "--results":
                        if (i + 1 >= args.Length)
                        {
                            Console.Error.WriteLine("Missing value for --results.");
                            return 2;
                        }
                        resultsPath = args[++i];
                        break;

                    default:
                        Console.Error.WriteLine("Unknown argument: " + args[i]);
                        PrintUsage();
                        return 2;
                }
            }

            return await ConsoleRunner.RunAsync(XmlToPoxTestSuites.All, resultsPath: resultsPath);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("XmlToPox Touchstone CLI runner");
            Console.WriteLine();
            Console.WriteLine("Usage: Test.Automated [--results <path>]");
            Console.WriteLine();
            Console.WriteLine("  --results <path>   Write JSON test results to <path>.");
            Console.WriteLine("  -h, --help         Show this help.");
        }
    }
}
