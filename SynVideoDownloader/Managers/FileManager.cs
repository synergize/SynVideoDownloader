using System;
using System.IO;
using SynVideoDownloader.Context;
using SynVideoDownloader.Helpers;

namespace SynVideoDownloader.Managers
{
    public class FileManager
    {
        /// <summary>
        /// Moves the downloaded file to the downloads folder of the windows user. 
        /// </summary>
        /// <param name="videoInfo"></param>
        private void MoveFileToDownloadsFolder(VideoInformation videoInfo)
        {
            try
            {
                var downloadsFolder = KnownFolders.GetPath(KnownFolder.Downloads);
                if (File.Exists($"{downloadsFolder}\\{videoInfo.FileName}"))
                {
                    Console.WriteLine($"{videoInfo.FileName} already exists within your Download Folder.");
                    Console.WriteLine($"Would you like to overwrite it? (y/n)");

                    if (ApplicationNavigation.DetermineYesOrNo())
                    {
                        File.Move(videoInfo.SourceLocation, downloadsFolder, true);
                    }
                    else
                    {
                        Console.WriteLine("Would you like to restart? (y/n)");
                        if (ApplicationNavigation.DetermineYesOrNo())
                        {
                            Console.Clear();
                            ApplicationNavigation.StartApplicationProcess();
                        }
                        else
                        {
                            ApplicationNavigation.CloseApplication(10);
                        }
                    }
                }
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
                throw;
            }

        }
    }
}
