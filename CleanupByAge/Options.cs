namespace CleanupByAge
{
    using CommandLine;

    public class Options
    {
        [Option('p', "paths", Required = false, HelpText = "Set the paths to process. Comma separated.")]
        public string Paths { get; set; }

        [Option('d', "days", Required = false, HelpText = "Set the age of files to remove. Uses Last Write Time", Default = 90)]
        public int Days { get; set; }

        [Option('f', "dryrun", Required = false, HelpText = "Set the age of files to remove. Uses Last Write Time", Default = false)]
        public bool DryRun { get; set; }
    }
}
