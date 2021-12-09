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
            Console.WriteLine("FACEBOOK VIDEO DOWNLOADER BY: Lucas00012");
            Console.Write("Please insert the video url that you want: ");
            var facebookPostUrl = Console.ReadLine();

            Console.Write("Please insert the output directory that you want: ");
            var outputDirectory = Console.ReadLine();

            try
            {
                var (firstVideoPartUrl, secondVideoPartUrl) = await ObtainVideoUrls(facebookPostUrl);

                var firstVideoPartFileName = FileDownload.DownloadFile(firstVideoPartUrl, outputDirectory);
                var secondVideoPartFileName = FileDownload.DownloadFile(secondVideoPartUrl, outputDirectory);

                FileDownload.MergeAudioAndVideo(firstVideoPartFileName, secondVideoPartFileName, outputDirectory);

                Console.Clear();
                Console.WriteLine("DOWNLOAD COMPLETED!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
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
                    chromeNetworkInterceptor.Finish();
                    return (videoDownloadUrls.ElementAt(0), videoDownloadUrls.ElementAt(1));
                }
            }
        }
    }
}