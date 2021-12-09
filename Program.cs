using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using RestSharp;
using System;
using System.Threading.Tasks;
using Network = OpenQA.Selenium.DevTools.V96.Network;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FacebookVideosDownloader.Helpers;

namespace FacebookVideosDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var facebookPostUrl = "https://www.facebook.com/100003559807264/videos/329669988989859/";
            var (firstVideoPartUrl, secondVideoPartUrl) = await ObtainVideoUrls(facebookPostUrl);
            var outputDirectory = @"D:\Temp";

            var firstVideoPartFileName = FileDownload.DownloadFile(firstVideoPartUrl, outputDirectory);
            var secondVideoPartFileName = FileDownload.DownloadFile(secondVideoPartUrl, outputDirectory);

            FileDownload.MergeAudioAndVideo(firstVideoPartFileName, secondVideoPartFileName, outputDirectory);
        }

        private static async Task<(string firstVideoPartUrl, string secondVideoPartUrl)> ObtainVideoUrls(string facebookPostUrl)
        {
            var videoDownloadUrls = new List<string>();

            EventHandler<Network.RequestInterceptedEventArgs> interceptor = (sender, e) =>
            {
                var url = Regex.Replace(e.Request.Url, @"bytestart=(\d+)&?|byteend=(\d+)&?", string.Empty);

                if (videoDownloadUrls.Count == 2)
                {
                    return;
                }

                if (url.StartsWith("https://video") && !videoDownloadUrls.Contains(url))
                {
                    videoDownloadUrls.Add(url);
                }
            };

            var chromeNetworkInterceptor = new ChromeNetworkInterceptor(facebookPostUrl, interceptor, Network.InterceptionStage.HeadersReceived, Network.ResourceType.XHR);
            await chromeNetworkInterceptor.Intercept();

            while (true)
            {
                if (videoDownloadUrls.Count == 2)
                {
                    chromeNetworkInterceptor.ChromeDriver.Close();
                    return (videoDownloadUrls.ElementAt(0), videoDownloadUrls.ElementAt(1));
                }
            }
        }
    }
}