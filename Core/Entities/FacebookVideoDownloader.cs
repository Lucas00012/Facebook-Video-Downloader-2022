using System.Collections.Generic;
using Network = OpenQA.Selenium.DevTools.V96.Network;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FacebookVideosDownloader.Core.Helpers;

namespace FacebookVideosDownloader.Core.Entities
{
    public class FacebookVideoDownloader
    {
        public async Task Download(string facebookPostUrl, string outputDirectory)
        {
            var (firstVideoPartUrl, secondVideoPartUrl) = await ObtainVideoPartUrls(facebookPostUrl);

            var firstVideoPartFileName = FileDownload.DownloadFile(firstVideoPartUrl, outputDirectory);
            var secondVideoPartFileName = FileDownload.DownloadFile(secondVideoPartUrl, outputDirectory);

            FileDownload.MergeAudioAndVideo(firstVideoPartFileName, secondVideoPartFileName, outputDirectory);
        }

        private async Task<(string, string)> ObtainVideoPartUrls(string facebookPostUrl)
        {
            var videoFilesUrls = new List<string>();

            var chromeNetworkInterceptor = new ChromeNetworkInterceptor();
            chromeNetworkInterceptor.Url = facebookPostUrl;
            chromeNetworkInterceptor.InterceptionStage = Network.InterceptionStage.HeadersReceived;
            chromeNetworkInterceptor.ResourceType = Network.ResourceType.XHR;

            await chromeNetworkInterceptor.Intercept((sender, e) =>
            {
                var url = Regex.Replace(e.Request.Url, @"bytestart=(\d+)&?|byteend=(\d+)&?", string.Empty);

                if (videoFilesUrls.Count == 2)
                    return;

                if (url.StartsWith("https://video") && !videoFilesUrls.Contains(url))
                    videoFilesUrls.Add(url);
            });

            while (videoFilesUrls.Count != 2) { }
            chromeNetworkInterceptor.Finish();

            return (videoFilesUrls.ElementAt(0), videoFilesUrls.ElementAt(1));
        }
    }
}
