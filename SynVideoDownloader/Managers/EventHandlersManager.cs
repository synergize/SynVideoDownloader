using System;
using System.Net;
using SynVideoDownloader.Helpers;

namespace SynVideoDownloader.Managers
{
    public static class EventHandlersManager
    {
        /// <summary>
        /// Event handler that triggers while a download is in progress via WebClient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void DownloadProgressEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\rDownload Progress {0}% ", e.ProgressPercentage);
        }

        /// <summary>
        /// Event Handler that triggers when a download via WebClient is complete. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void DownloadCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.Clear();
                Console.WriteLine("Download Complete!");
                Console.WriteLine("Would you like to download another? (y/n)");
                if (ApplicationNavigation.DetermineYesOrNo())
                {
                    Console.Clear();
                    ApplicationNavigation.StartApplicationProcess();
                }
                else
                {
                    ApplicationNavigation.CloseApplication();
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"Download failed due to {e.Error?.Message}.");
                ApplicationNavigation.DetermineRetryApplication();
            }
        }
    }
}
