using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V96.DevToolsSessionDomains;
using Network = OpenQA.Selenium.DevTools.V96.Network;

namespace FacebookVideosDownloader.Helpers
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
        }

        public string Url { get; private set; }
        private EventHandler<Network.RequestInterceptedEventArgs> Interceptor { get; set; }
        private Network.InterceptionStage InterceptionStage { get; set; }
        private Network.ResourceType ResourceType { get; set; }
        private ChromeDriver ChromeDriver { get; set; }

        public async Task Intercept()
        {
            var service = ChromeDriverService.CreateDefaultService();
            var options = new ChromeOptions();
            var driver = new ChromeDriver(service, options);

            var session = driver.GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<DevToolsSessionDomains>();

            await domains.Network.Enable(new Network.EnableCommandSettings());

            var requestPattern = new Network.RequestPattern();
            requestPattern.InterceptionStage = InterceptionStage;
            requestPattern.ResourceType = ResourceType;

            var setRequestInterceptionCommandSettings = new Network.SetRequestInterceptionCommandSettings();
            setRequestInterceptionCommandSettings.Patterns = new Network.RequestPattern[] { requestPattern };

            await domains.Network.SetRequestInterception(setRequestInterceptionCommandSettings);
            domains.Network.RequestIntercepted += Interceptor;

            driver.Url = Url;

            ChromeDriver = driver;
        }

        public void Finish()
        {
            ChromeDriver.Close();
        }
    }
}
