using FacebookVideosDownloader.Core.Enums;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V103.DevToolsSessionDomains;
using Network = OpenQA.Selenium.DevTools.V103.Network;
using Fetch = OpenQA.Selenium.DevTools.V103.Fetch;

namespace FacebookVideosDownloader.Core.Entities
{
    public class ChromeNetworkInterceptor : IDisposable
    {
        public string Url { get; set; }
        public Fetch.RequestStage RequestStage { get; set; }
        public Network.ResourceType ResourceType { get; set; }

        private ChromeDriver ChromeDriver { get; set; }

        public ChromeNetworkInterceptor()
        {
            ChromeDriver = (ChromeDriver)WebDriverFactory.CreateWebDriver(Browser.Chrome);
        }

        public async Task Intercept(EventHandler<Fetch.RequestPausedEventArgs> interceptor)
        {
            var session = ChromeDriver.GetDevToolsSession();
            var fetch = session.GetVersionSpecificDomains<DevToolsSessionDomains>().Fetch;

            var enableCommandSettings = GetEnableCommandSettings();

            await fetch.Enable(enableCommandSettings);
            fetch.RequestPaused += interceptor;

            ChromeDriver.Navigate().GoToUrl(Url);
        }

        private Fetch.EnableCommandSettings GetEnableCommandSettings()
        {
            var enableCommandSettings = new Fetch.EnableCommandSettings();

            var patterns = new Fetch.RequestPattern[]
            {
                new Fetch.RequestPattern
                {
                    RequestStage = RequestStage,
                    ResourceType = ResourceType,
                }
            };

            enableCommandSettings.Patterns = patterns;
            return enableCommandSettings;
        }

        public void Close()
        {
            ChromeDriver.Close();
        }

        public void Dispose()
        {
            ChromeDriver.Quit();
        }
    }
}
