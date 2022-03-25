using System.Collections.Generic;
using Network = OpenQA.Selenium.DevTools.V96.Network;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium;

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

            var seleniumNavigation = new SeleniumNavigation(chromeNetworkInterceptor.ChromeDriver);
            seleniumNavigation.Navigate(facebookPostUrl);

            var videoContainsAudio = VideoContainsAudio(seleniumNavigation);

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

        public bool VideoContainsAudio(SeleniumNavigation seleniumNavigation)
        {
            var container = seleniumNavigation.GetElement(By.CssSelector("div.oajrlxb2.rq0escxv.p7hjln8o.i1ao9s8h.esuyzwwr.f1sip0of.n00je7tq.arfg74bv.qs9ysxi8.k77z8yql.l9j0dhe7.abiwlrkh.p8dawk7l.g5ia77u1.gcieejh5.bn081pho.humdl8nn.izx4hr6d.nhd2j8a9.q9uorilb.jnigpg78.qjjbsfad.fv0vnmcu.w0hvl6rk.ggphbty4.byekypgc.jb3vyjys.rz4wbd8a.qt6c0cv9.a8nywdso.i2p6rm4e.lzcic4wl"));
            var songIconElement = container.FindElement(By.CssSelector("i.hu5pjgll.eb18blue"));

            var x = songIconElement.GetCssValue("background-position");
            var notMuttedIcon = songIconElement.GetCssValue("background-position") != "-42px -135px";

            return notMuttedIcon;
        }
    }
}
