using System;
using System.IO;
using System.Threading.Tasks;
using SynVideoDownloader.Context;
using SynVideoDownloader.Helpers;

namespace SynVideoDownloader
{
    internal class Program
    {
        private static VideoInformation _videoInfo;

        static void Main(string[] args) => StartWebsiteScrape().GetAwaiter().GetResult();
        private static async Task StartWebsiteScrape()
        {

            try
            {
                _videoInfo = ApplicationNavigation.StartApplicationProcess();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application exploded. Please locate ErrorReport.txt and send it to my developer.");
                var filePath = $"{Environment.CurrentDirectory}\\ErrorReport.txt";
                await using (var writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.UtcNow.ToLongDateString());
                    writer.WriteLine($"Video URL: {_videoInfo.VideoUrl ?? ""}");
                    writer.WriteLine($"File Name: {_videoInfo.FileName ?? ""}");
                    writer.WriteLine("Operating System Version: {0}", Environment.OSVersion.Version);
                    writer.WriteLine("Operating System Platform: {0}", Environment.OSVersion.Platform);
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
                throw;
            }
        }
    }
}
