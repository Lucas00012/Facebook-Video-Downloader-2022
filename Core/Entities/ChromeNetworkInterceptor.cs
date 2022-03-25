using FacebookVideosDownloader.Core.Enums;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V96.DevToolsSessionDomains;
using Network = OpenQA.Selenium.DevTools.V96.Network;

namespace FacebookVideosDownloader.Core.Entities
{
    public class ChromeNetworkInterceptor
    {
        public ChromeNetworkInterceptor()
        {
            ChromeDriver = (ChromeDriver)WebDriverFactory.CreateWebDriver(Browser.Chrome, false);
        }

        public string Url { get; set; }
        public Network.InterceptionStage InterceptionStage { get; set; }
        public Network.ResourceType ResourceType { get; set; }

        private ChromeDriver ChromeDriver { get; set; }

        public async Task Intercept(EventHandler<Network.RequestInterceptedEventArgs> interceptor)
        {
            var session = ChromeDriver.GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<DevToolsSessionDomains>();

            var requestInterceptionCommandSettings = GetRequestInterceptionSettings();

            await domains.Network.Enable(new Network.EnableCommandSettings());
            await domains.Network.SetRequestInterception(requestInterceptionCommandSettings);
            domains.Network.RequestIntercepted += interceptor;

            ChromeDriver.Navigate().GoToUrl(Url);
        }

        public void Finish()
        {
            ChromeDriver.Close();
        }

        private Network.SetRequestInterceptionCommandSettings GetRequestInterceptionSettings()
        {
            var requestPattern = new Network.RequestPattern();
            requestPattern.InterceptionStage = InterceptionStage;
            requestPattern.ResourceType = ResourceType;

            var setRequestInterceptionCommandSettings = new Network.SetRequestInterceptionCommandSettings();
            setRequestInterceptionCommandSettings.Patterns = new Network.RequestPattern[] { requestPattern };

            return setRequestInterceptionCommandSettings;
        }
    }
}
