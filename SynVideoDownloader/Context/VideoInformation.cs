using SynVideoDownloader.Enums;

namespace SynVideoDownloader.Context
{
    public class VideoInformation
    {
        public string VideoUrl { get; set; }
        public string FileName { get; set; }
        public string SourceLocation { get; set; }
        public VideoSource Source { get; set; }

        public VideoInformation(string videoUrl, string fileName, VideoSource source)
        {
            VideoUrl = videoUrl;
            FileName = fileName;
            Source = source;
        }

        public VideoInformation()
        {

        }
    }
}
