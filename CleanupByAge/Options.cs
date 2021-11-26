namespace CleanupByAge;

public class Options
{
    [Option('p', "paths", Required = false, HelpText = "Set the paths to process. Comma separated.")]
    public string Paths { get; set; }

    [Option('d', "days", Required = false, HelpText = "Set the age of files to remove. Uses Last Write Time.", Default = 90)]
    public int Days { get; set; }

    [Option('f', "dryrun", Required = false, HelpText = "Set whether to perform a dry run delete or not.", Default = false)]
    public bool DryRun { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Set message logging to verbose.", Default = false)]
    public bool Verbose { get; set; }

    [Option('m', "multithread", Required = false, HelpText = "Set deletion to operate in parallel across provided paths", Default = false)]
    public bool MultiThread { get; set; }
}