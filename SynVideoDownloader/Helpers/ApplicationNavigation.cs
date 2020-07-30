using System;
using System.Diagnostics;
using System.Threading;
using SynVideoDownloader.Context;
using SynVideoDownloader.Managers;

namespace SynVideoDownloader.Helpers
{
    public static class ApplicationNavigation
    {
        /// <summary>
        /// Logic to determine if the user wants to retry. 
        /// </summary>
        public static void DetermineRetryApplication()
        {
            Console.WriteLine("Would you like to try again? (y/n)");

            if (DetermineYesOrNo())
            {
                Console.Clear();
                StartApplicationProcess();
            }
            else
            {
                CloseApplication(10);
            }
        }

        /// <summary>
        /// Close application after defined amount of time. Will also count down by the second until it closes.
        /// </summary>
        /// <param name="duration"></param>
        public static void CloseApplication(int duration = 30)
        {
            var closeDuration = new TimeSpan(0, 0, 0, duration);
            var iteration = closeDuration.Seconds;
            var closeTimer = new Stopwatch();

            closeTimer.Start();
            while (closeTimer.Elapsed < closeDuration)
            {
                Console.Write("\rThis program will self destruct {0} seconds ", iteration);
                --iteration;
                Thread.Sleep(1000);
            }
            closeTimer.Stop();
            Environment.Exit(0);
        }

        /// <summary>
        /// Logic to handle the user entering Yes or No when asked.
        /// </summary>
        /// <returns></returns>
        public static bool DetermineYesOrNo()
        {
            var retryAnswer = Console.ReadKey(true);
            switch (retryAnswer.Key)
            {
                case ConsoleKey.Y:
                    return true;
                case ConsoleKey.N:
                    return false;
                default:
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine("\rPlease enter Y or N                 ");
                    return DetermineYesOrNo();
            }
        }

        /// <summary>
        /// Logic to handle the inputs from the user to determine the video we're going to download for them.
        /// </summary>
        /// <returns></returns>
        public static VideoInformation StartApplicationProcess()
        {
            var videoInfo = new VideoInformation();
            while (true)
            {
                Console.WriteLine($"===========================================================================");
                Console.WriteLine("Application developed by Coaction. Currently only tested against streamable.");
                Console.WriteLine("                  However, other websites might work.                       ");
                Console.WriteLine($"===========================================================================");
                Console.WriteLine("\n");
                Console.WriteLine("Enter the URL of the video you'd like to download.");

                videoInfo.VideoUrl = Console.ReadLine() ?? "";
                var validateHyperlink = Uri.TryCreate(videoInfo.VideoUrl, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!validateHyperlink)
                {
                    Console.Clear();
                    Console.WriteLine("Entry not a valid website. Enter a valid web address.");
                    Console.WriteLine("\n");
                    continue;
                }

                Console.WriteLine("Enter the name of the video you'd like to download. Do not provide a file extension.");
                videoInfo.FileName = $"{Console.ReadLine()}";
                var downloadManager = new VideoDownloadManager(videoInfo);
                if (uriResult.Host == "www.youtube.com")
                {
                    // youtube logic
                }
                else
                {
                    downloadManager.DownloadStreamableVideo();
                }
                break;
            }

            return videoInfo;
        }
    }
}
