using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium;
using System.Threading;
using Network = OpenQA.Selenium.DevTools.V96.Network;
using Fetch = OpenQA.Selenium.DevTools.V96.Fetch;
using FacebookVideosDownloader.Core.Enums;
using System;

namespace FacebookVideosDownloader.Core.Entities
{
    public class FacebookVideoDownloader : IDisposable
    {
        public FacebookVideoDownloader()
        {
            Scraper = new SeleniumScraperEngine(Browser.Chrome);
            Interceptor = new ChromeNetworkInterceptor { RequestStage = Fetch.RequestStage.Response, ResourceType = Network.ResourceType.XHR };
        }

        private SeleniumScraperEngine Scraper { get; set; }
        private ChromeNetworkInterceptor Interceptor { get; set; }

        public async Task Download(string facebookPostUrl, string outputDirectory)
        {
            var videoPartsUrls = await ObtainVideoPartUrls(facebookPostUrl);
            var firstVideoPartFileName = FileDownload.DownloadFileAndSave(videoPartsUrls.ElementAt(0), outputDirectory);

            if (videoPartsUrls.Count == 1)
                return;

            var secondVideoPartFileName = FileDownload.DownloadFileAndSave(videoPartsUrls.ElementAt(1), outputDirectory);
            FileDownload.MergeAudioAndVideoAndSave(firstVideoPartFileName, secondVideoPartFileName, outputDirectory);
        }

        private async Task<List<string>> ObtainVideoPartUrls(string facebookPostUrl)
        {
            Interceptor.Url = facebookPostUrl;

            var videoContainsAudio = VideoContainsAudio(facebookPostUrl);

            var videoFilesUrls = new List<string>();
            var waitHandle = new AutoResetEvent(false);

            await Interceptor.Intercept((sender, e) =>
            {
                var url = Regex.Replace(e.Request.Url, @"bytestart=(\d+)&?|byteend=(\d+)&?", string.Empty);

                if (videoContainsAudio && videoFilesUrls.Count == 2 || !videoContainsAudio && videoFilesUrls.Count == 1)
                {
                    waitHandle.Set();
                    return;
                }

                if (url.StartsWith("https://video") && !videoFilesUrls.Contains(url))
                    videoFilesUrls.Add(url);
            });

            waitHandle.WaitOne();

            return videoFilesUrls;
        }

        public bool VideoContainsAudio(string facebookPostUrl)
        {
            Scraper.Navigate(facebookPostUrl);

            var container = Scraper.GetElementByCssSelector("div.l9j0dhe7.pcp91wgn.iuny7tx3.p8fzw8mz.discj3wi.o7xrwllt.q9uorilb.nhd2j8a9");
            var songIcon = container.FindElement(By.XPath(".//span/span/span/div/i"));

            var songIconPosition = songIcon.GetCssValue("background-position");
            var songIconNotBlocked = songIconPosition != "-25px -264px";

            return songIconNotBlocked;
        }

        public void Dispose()
        {
            Interceptor.Dispose();
            Scraper.Dispose();
        }
    }
}
