using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SynVideoDownloader
{
    class Program
    {
        private static string VideoUrl { get; set; }
        private static string FileName { get; set; }
        private WebClient _webClient;

        static void Main(string[] args) => new Program().StartWebsiteScrape().GetAwaiter().GetResult();
        private async Task StartWebsiteScrape()
        {

            try
            {
                UserInformationDisplay();

                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(new Uri(VideoUrl));

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var urlFound = htmlDocument.DocumentNode.Descendants("meta").Where(x => x.GetAttributeValue("property", "").Equals("og:video:url")).ToList();
                if (urlFound.Count == 0)
                {
                    var videoUrl = urlFound[0].GetAttributeValue("content", "");
                    _webClient = new WebClient();
                    _webClient.DownloadFileAsync(new Uri(videoUrl), FileName);
                    _webClient.DownloadProgressChanged += DownloadProgressEventHandler;
                    _webClient.DownloadFileCompleted += DownloadCompletedEventHandler;
                }

                if (urlFound.Count == 0)
                {
                    Console.WriteLine($"No videos found. Website either not supported or contains no video to download.");
                    Console.WriteLine($"Would you like to try again? (y/n)");
                    DetermineRetryApplication();
                }

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
                    writer.WriteLine($"Video URL: {VideoUrl}");
                    writer.WriteLine($"File Name: {FileName}");
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

        private void DownloadProgressEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\rDownload Progress {0}% ", e.ProgressPercentage);
        }

        private void DownloadCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.Clear();
                Console.WriteLine($"Download Complete!");
                Console.WriteLine($"Downloaded From: {VideoUrl}");
                Console.WriteLine($"Downloaded File Name: {FileName}");
                CloseApplication();
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"Download failed due to {e.Error?.Message}.");
                DetermineRetryApplication();
            }
        }

        private static void UserInformationDisplay()
        {
            while (true)
            {
                Console.WriteLine($"===========================================================================");
                Console.WriteLine("Application developed by Coaction. Currently only tested against streamable.");
                Console.WriteLine("                  However, other websites might work.                       ");
                Console.WriteLine($"===========================================================================");
                Console.WriteLine("\n");
                Console.WriteLine("Enter the URL of the video you'd like to download.");

                VideoUrl = Console.ReadLine() ?? "";
                var validateHyperlink = Uri.TryCreate(VideoUrl, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!validateHyperlink)
                {
                    Console.Clear();
                    Console.WriteLine("Entry not a valid website. Enter a valid web address.");
                    Console.WriteLine("\n");
                    continue;
                }
                break;
            }

            Console.WriteLine("Enter the name of the video you'd like to download. Do not provide a file extension.");
            FileName = $"{Console.ReadLine()}.mp4";
        }

        private async void DetermineRetryApplication(bool initialRetry = true)
        {
            if (initialRetry)
            {
                Console.WriteLine("Would you like to try again? (y/n)");
            }
            var retryAnswer = Console.ReadKey(true);
            switch (retryAnswer.Key)
            {
                case ConsoleKey.Y:
                    Console.Clear();
                    await StartWebsiteScrape();
                    break;
                case ConsoleKey.N:
                    CloseApplication(10);
                    break;
                default:
                    DetermineRetryApplication(false);
                    break;
            }
        }

        private void CloseApplication(int duration = 30)
        {
            var closeDuration = new TimeSpan(0, 0, 0, duration);
            var iteration = closeDuration.Seconds;
            var closeTimer = new Stopwatch();

            closeTimer.Start();
            while (closeTimer.Elapsed < closeDuration)
            {
                Console.Write("\rThis program will close in {0} seconds ", iteration);
                --iteration;
                Thread.Sleep(1000);
            }
            closeTimer.Stop();
            Environment.Exit(0);
        }
    }
}
