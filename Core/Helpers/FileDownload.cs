using RestSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace FacebookVideosDownloader.Core.Helpers
{
    public static class FileDownload
    {
        public static void MergeAudioAndVideo(string firstVideoPartFileName, string secondVideoPartFileName, string outputDirectory)
        {
            var fileName = $"{Guid.NewGuid()}.mp4";

            string args = $"/c ffmpeg -i \"{firstVideoPartFileName}\" -i \"{secondVideoPartFileName}\" -shortest {fileName}";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = @"" + outputDirectory;
            startInfo.Arguments = args;

            using Process exeProcess = Process.Start(startInfo);
            exeProcess.WaitForExit();

            File.Delete(Path.Combine(outputDirectory, firstVideoPartFileName));
            File.Delete(Path.Combine(outputDirectory, secondVideoPartFileName));

            exeProcess.Close();
        }

        public static string DownloadFile(string url, string outputDirectory)
        {
            var client = new RestClient();
            var request = new RestRequest(url, Method.GET);

            var fileName = $"{Guid.NewGuid()}.mp4";
            var bytes = client.DownloadData(request);

            File.WriteAllBytes(Path.Combine(outputDirectory, fileName), bytes);

            return fileName;
        }
    }
}
