using FacebookVideosDownloader.Core.Enums;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V96.DevToolsSessionDomains;
using Network = OpenQA.Selenium.DevTools.V96.Network;

namespace FacebookVideosDownloader.Core.Entities
{
    public class ChromeNetworkInterceptor
    {
        public ChromeNetworkInterceptor(
            string url,
            EventHandler<Network.RequestInterceptedEventArgs> interceptor,
            Network.InterceptionStage interceptionStage,
            Network.ResourceType resourceType)
        {
            Url = url;

            Interceptor = interceptor;
            ResourceType = resourceType;
            InterceptionStage = interceptionStage;

            ChromeDriver = (ChromeDriver)WebDriverFactory.CreateWebDriver(Browser.Chrome);
        }

        public string Url { get; private set; }

        private EventHandler<Network.RequestInterceptedEventArgs> Interceptor { get; set; }
        private Network.InterceptionStage InterceptionStage { get; set; }
        private Network.ResourceType ResourceType { get; set; }

        private ChromeDriver ChromeDriver { get; set; }

        public async Task Intercept()
        {
            var session = ChromeDriver.GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<DevToolsSessionDomains>();

            await domains.Network.Enable(new Network.EnableCommandSettings());

            var requestInterceptionCommandSettings = GetRequestInterceptionSettings();

            await domains.Network.SetRequestInterception(requestInterceptionCommandSettings);
            domains.Network.RequestIntercepted += Interceptor;

            ChromeDriver.Navigate().GoToUrl(Url);
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

        public void Finish()
        {
            ChromeDriver.Close();
        }
    }
}
