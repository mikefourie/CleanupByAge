namespace CleanupByAge
{
    using System;
    using System.IO;
    using System.Linq;
    using CommandLine;

    public class Program
    {
        private static Options programOptions = new Options();

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(o =>
                       {
                           programOptions = o;
                       });

            int days = programOptions.Days;
            string[] paths = programOptions.Paths.Split(",", StringSplitOptions.RemoveEmptyEntries);

            int filecount = 1, dircount = 1;
            long length = 0;

            // -d 70 -p "N:\roslogs","N:\rosbags","N:\coredumps","N:\px4logs"
            foreach (string path in paths)
            {
                try
                {
                    foreach (string rootdirdirectory in Directory.GetDirectories(path))
                    {
                        Console.WriteLine($"Scanning - {rootdirdirectory}");

                        foreach (string file in Directory.EnumerateFiles(rootdirdirectory, "*.*", SearchOption.AllDirectories).Where(path => File.GetLastWriteTime(path) < DateTime.Now.AddDays(-days)).ToList())
                        {
                            FileInfo f = new FileInfo(file);
                            length += f.Length;
                            Console.WriteLine($"{filecount++} {file} {f.LastWriteTime} {BytesToString(f.Length)}");
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

            Console.WriteLine($"dircount {--dircount}, filecount {--filecount}, space {BytesToString(length)}");
            Console.ReadLine();
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
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}