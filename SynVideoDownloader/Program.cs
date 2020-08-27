using System;
using System.Threading.Tasks;
using SynVideoDownloader.Helpers;
using SynVideoDownloader.Logger;

namespace SynVideoDownloader
{
    internal class Program
    {
        static void Main(string[] args) => StartProgram().GetAwaiter().GetResult();
        private static async Task StartProgram()
        {
            try
            {
                ApplicationNavigation.StartApplicationProcess();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application exploded. Please locate ErrorReport.txt and send it to my developer.");
                Log.LogExceptionAsync(ex, ApplicationNavigation.VideoInfo);
                ApplicationNavigation.RestartApplication();
            }
        }
    }
}
