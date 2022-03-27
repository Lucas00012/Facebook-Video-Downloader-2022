using RestSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace FacebookVideosDownloader.Core.Helpers
{
    public static class FileDownload
    {
        public static void MergeAudioAndVideoAndSave(string firstVideoPartFileName, string secondVideoPartFileName, string outputDirectory)
        {
            var fileName = $"video_{Guid.NewGuid()}.mp4";

            var startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg", "ffmpeg.exe");
            startInfo.WorkingDirectory = $@"{outputDirectory}";
            startInfo.Arguments = $"-i \"{firstVideoPartFileName}\" -i \"{secondVideoPartFileName}\" -shortest {fileName}";

            using var exeProcess = Process.Start(startInfo);
            exeProcess.WaitForExit();
            exeProcess.Close();

            File.Delete(Path.Combine(outputDirectory, firstVideoPartFileName));
            File.Delete(Path.Combine(outputDirectory, secondVideoPartFileName));
        }

        public static string DownloadFileAndSave(string url, string outputDirectory)
        {
            var client = new RestClient();
            var request = new RestRequest(url, Method.GET);

            var fileName = $"video_{Guid.NewGuid()}.mp4";
            var bytes = client.DownloadData(request);

            File.WriteAllBytes(Path.Combine(outputDirectory, fileName), bytes);

            return fileName;
        }
    }
}
