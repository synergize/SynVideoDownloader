using System;
using System.IO;
using SynVideoDownloader.Context;

namespace SynVideoDownloader.Logger
{
    public static class Log
    {
        public static void LogExceptionAsync(Exception ex, VideoInformation videoInfo)
        {
            videoInfo ??= new VideoInformation();

            var filePath = $"{Environment.CurrentDirectory}\\ErrorReport.txt";
            using var writer = new StreamWriter(filePath, true);
            writer.WriteLine("-----------------------------------------------------------------------------");
            writer.WriteLine("Date : " + DateTime.UtcNow.ToLongDateString());
            writer.WriteLine($"Video URL: {videoInfo.VideoUrl ?? "URL Not Found"}");
            writer.WriteLine($"File Name: {videoInfo.FileName ?? "File Name Not Found"}");
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

        public static void LogExceptionAsync(Exception ex)
        {
            LogExceptionAsync(ex, new VideoInformation());
        }
    }
}
