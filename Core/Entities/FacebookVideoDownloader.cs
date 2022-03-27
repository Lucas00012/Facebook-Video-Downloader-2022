using System.Collections.Generic;
using Network = OpenQA.Selenium.DevTools.V96.Network;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace FacebookVideosDownloader.Core.Entities
{
    public class FacebookVideoDownloader
    {
        public async Task Download(string facebookPostUrl, string outputDirectory)
        {
            var videoPartsUrls = await ObtainVideoPartUrls(facebookPostUrl);
            var firstVideoPartFileName = FileDownload.DownloadFileAndSave(videoPartsUrls.ElementAt(0), outputDirectory);

            if (videoPartsUrls.Count == 2)
            {
                var secondVideoPartFileName = FileDownload.DownloadFileAndSave(videoPartsUrls.ElementAt(1), outputDirectory);
                FileDownload.MergeAudioAndVideoAndSave(firstVideoPartFileName, secondVideoPartFileName, outputDirectory);
            }
        }

        private async Task<List<string>> ObtainVideoPartUrls(string facebookPostUrl)
        {
            var videoFilesUrls = new List<string>();

            var chromeNetworkInterceptor = new ChromeNetworkInterceptor();
            chromeNetworkInterceptor.Url = facebookPostUrl;
            chromeNetworkInterceptor.InterceptionStage = Network.InterceptionStage.HeadersReceived;
            chromeNetworkInterceptor.ResourceType = Network.ResourceType.XHR;

            bool? videoContainsAudio = null;
            var waitHandle = new AutoResetEvent(false);

            await chromeNetworkInterceptor.Intercept((sender, e) =>
            {
                videoContainsAudio = !videoContainsAudio.HasValue ? VideoContainsAudio(chromeNetworkInterceptor) : videoContainsAudio.Value;
                var url = Regex.Replace(e.Request.Url, @"bytestart=(\d+)&?|byteend=(\d+)&?", string.Empty);

                if (videoContainsAudio.Value && videoFilesUrls.Count == 2 || !videoContainsAudio.Value && videoFilesUrls.Count == 1)
                {
                    waitHandle.Set();
                    return;
                }

                if (url.StartsWith("https://video") && !videoFilesUrls.Contains(url))
                    videoFilesUrls.Add(url);
            });

            waitHandle.WaitOne();
            chromeNetworkInterceptor.Finish();

            return videoFilesUrls;
        }

        public bool VideoContainsAudio(ChromeNetworkInterceptor chromeNetworkInterceptor)
        {
            var container = chromeNetworkInterceptor.FindElement(By.CssSelector("div.l9j0dhe7.pcp91wgn.iuny7tx3.p8fzw8mz.discj3wi.o7xrwllt.q9uorilb.nhd2j8a9"));
            var songIcon = container.FindElement(By.XPath(".//span/span/span/div/i"));
            var backgroundPosition = songIcon.GetCssValue("background-position");

            //properties that set the blocked audio icon in multi-icons image
            var notBlockedSongIcon = backgroundPosition != "-25px -264px";

            return notBlockedSongIcon;
        }
    }
}
