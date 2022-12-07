namespace CleanupByAge;

public class Program
{
    private static Options programOptions = new ();

    public static void Main(string[] args)
    {
        DateTime start = DateTime.Now;
        Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        programOptions = o;
                    });

        int days = programOptions.Days;
        string[] paths = programOptions.Paths.Split(",", StringSplitOptions.RemoveEmptyEntries);

        int filecount = 1, dircount = 1;
        long length = 0;

        if (!programOptions.MultiThread || paths.Length == 1)
        {
            foreach (string scanPath in paths)
            {
                try
                {
                    foreach (string rootdirdirectory in Directory.GetDirectories(scanPath))
                    {
                        Console.WriteLine(!programOptions.DryRun ? $"Scanning - {rootdirdirectory}" : $"Scanning - {rootdirdirectory} - DRY RUN");
                        foreach (string file in Directory.EnumerateFiles(rootdirdirectory, "*.*", SearchOption.AllDirectories).AsParallel().Where(path => File.GetLastWriteTime(path) < DateTime.Now.AddDays(-days)).ToList())
                        {
                            FileInfo f = new (file);
                            length += f.Length;
                            filecount++;
                            if (programOptions.Verbose)
                            {
                                Console.WriteLine($"{filecount} {file} {f.LastWriteTime} {BytesToString(f.Length)}");
                            }

                            if (!programOptions.DryRun)
                            {
                                File.Delete(file);
                            }
                        }

                        foreach (string directory in Directory.GetDirectories(rootdirdirectory, "*.*", SearchOption.AllDirectories).Where(d => Directory.GetFiles(d).Length == 0 && Directory.GetDirectories(d).Length == 0))
                        {
                            Console.WriteLine($"{dircount++} {directory}");
                            if (!programOptions.DryRun)
                            {
                                Directory.Delete(directory, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OH DEAR - {ex}");
                }
            }

            TimeSpan t = DateTime.Now - start;
            Console.WriteLine($"dircount {--dircount}, filecount {--filecount}, space {BytesToString(length)}. {t.TotalMinutes}m: {t.Seconds}s");
        }
        else
        {
            Console.WriteLine($"Processing {paths.Length} paths in parallel...");

            Parallel.ForEach(paths, scanPath =>
            {
                try
                {
                    foreach (string rootdirdirectory in Directory.GetDirectories(scanPath))
                    {
                        Console.WriteLine(!programOptions.DryRun ? $"Scanning - {rootdirdirectory}" : $"Scanning - {rootdirdirectory} - DRY RUN");
                        foreach (string file in Directory.EnumerateFiles(rootdirdirectory, "*.*", SearchOption.AllDirectories).Where(path => File.GetLastWriteTime(path) < DateTime.Now.AddDays(-days)).ToList())
                        {
                            FileInfo f = new (file);
                            length += f.Length;
                            filecount++;
                            if (programOptions.Verbose)
                            {
                                Console.WriteLine($"{filecount} {file} {f.LastWriteTime} {BytesToString(f.Length)}");
                            }

                            if (!programOptions.DryRun)
                            {
                                File.Delete(file);
                            }
                        }

                        foreach (string directory in Directory.GetDirectories(rootdirdirectory, "*.*", SearchOption.AllDirectories).Where(d => Directory.GetFiles(d).Length == 0 && Directory.GetDirectories(d).Length == 0))
                        {
                            Console.WriteLine($"{dircount++} {directory}");
                            if (!programOptions.DryRun)
                            {
                                Directory.Delete(directory, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OH DEAR - {ex}");
                }
            });
            TimeSpan t = DateTime.Now - start;
            Console.WriteLine($"dircount {--dircount}, filecount {--filecount}, space {BytesToString(length)}. {t.TotalMinutes}m: {t.Seconds}s");
        }
    }

    private static string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        if (byteCount == 0)
        {
            return "0" + suf[0];
        }

        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num) + suf[place];
    }
}