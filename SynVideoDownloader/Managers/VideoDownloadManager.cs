using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using SynVideoDownloader.Context;
using SynVideoDownloader.Helpers;

namespace SynVideoDownloader.Managers
{
    public class VideoDownloadManager 
    {
        private VideoInformation VideoInfo { get; }

        public VideoDownloadManager(VideoInformation videoInfo)
        {
            VideoInfo = videoInfo;
        }

        /// <summary>
        /// Used HtmlAgilityPack to scrape Streamable and acquire a download URL for the video, then downloads it.
        /// </summary>
        public async void DownloadStreamableVideo()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(new Uri(VideoInfo.VideoUrl));

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var urlFound = htmlDocument.DocumentNode.Descendants("meta").Where(x => x.GetAttributeValue("property", "").Equals("og:video:url")).ToList();
            if (urlFound.Count == 1)
            {
                var videoUrl = urlFound[0].GetAttributeValue("content", "");
                var webClient = new WebClient();
                webClient.DownloadFileAsync(new Uri(videoUrl), $"{VideoInfo.FileName}.mp4");
                webClient.DownloadProgressChanged += EventHandlersManager.DownloadProgressEventHandler;
                webClient.DownloadFileCompleted += EventHandlersManager.DownloadCompletedEventHandler;
            }

            if (urlFound.Count == 0)
            {
                Console.WriteLine($"No videos found. Website either not supported or contains no video to download.");
                ApplicationNavigation.DetermineRetryApplication();
            }
        }
    }
}
