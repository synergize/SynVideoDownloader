using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using OpenQA.Selenium;
using SynVideoDownloader.Context;
using SynVideoDownloader.Enums;
using SynVideoDownloader.Helpers;
using SynVideoDownloader.Logger;
using SynVideoDownloader.Selenium;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Threading.Tasks;
using YoutubeExplode.Videos;


namespace SynVideoDownloader.Managers
{
    public class VideoDownloadManager : SeleniumBase
    {
        private VideoInformation VideoInfo { get; }

        public VideoDownloadManager(VideoInformation videoInfo)
        {
            VideoInfo = videoInfo;
        }

        /// <summary>
        /// Used HtmlAgilityPack to scrape Streamable and acquire a download URL for the video, then downloads it.
        /// </summary>
        public async void DownloadStreamableVideo(VideoSource source)
        {
            string videoUrl = null;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(new Uri(VideoInfo.VideoUrl));
            switch (source)
            {
                case VideoSource.Streamable:
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    List<HtmlNode> urlFound;

                    urlFound = htmlDocument.DocumentNode.Descendants("meta").Where(x => x.GetAttributeValue("property", "").Equals("og:video:url")).ToList();

                    switch (urlFound.Count)
                    {
                        case 1:
                        {
                            videoUrl = urlFound.FirstOrDefault()?.GetAttributeValue("content", "");
                            break;
                        }
                        case 0:
                            Console.WriteLine($"No videos found. Website either not supported or contains no video to download.");
                            ApplicationNavigation.DetermineRetryApplication();
                            break;
                    }
                    break;
                case VideoSource.TwitchClip:
                    try
                    {
                        WebDriver.Navigate().GoToUrl(VideoInfo.VideoUrl);
                        videoUrl = WebDriver.FindElement(By.XPath("//video")).GetAttribute("src");
                        TearDown();
                    }
                    catch (Exception e)
                    {
                        TearDown();
                        Console.WriteLine($"There was an issue trying to download your twitch clip: {e.Message}");
                        Log.LogExceptionAsync(e);
                        ApplicationNavigation.DetermineRetryApplication();
                    }
                    break;
                case VideoSource.Youtube:
                    var youtube = new YoutubeClient(httpClient);
                    var video = await youtube.Videos.GetAsync(VideoInfo.VideoUrl); 
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                    
                    // ...or highest quality MP4 video-only stream
                    var streamInfo = streamManifest
                        .GetMuxed().WithHighestVideoQuality();

                    if (streamInfo != null)
                    {
                        // Get the actual stream
                        var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                        // Download the stream to file
                        IProgress<double> progress = new Progress<double>();
                        var youtubeDownload = youtube.Videos.Streams.DownloadAsync(streamInfo,
                                $"{Environment.CurrentDirectory}\\{VideoInfo.FileName}.{streamInfo.Container}",
                                progress).ConfigureAwait(true).GetAwaiter();
                        youtubeDownload.OnCompleted(EventHandlersManager.DownloadCompleteMessaging);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, "Currently unsupported.");
            }

            if (source != VideoSource.Youtube)
            {
                StartDownload(videoUrl);
            }
        }

        private void StartDownload(string videoUrl)
        {
            var webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(videoUrl), $"{VideoInfo.FileName}.mp4");
            webClient.DownloadProgressChanged += EventHandlersManager.DownloadProgressEventHandler;
            webClient.DownloadFileCompleted += EventHandlersManager.DownloadCompletedEventHandler;
        }
    }
}
