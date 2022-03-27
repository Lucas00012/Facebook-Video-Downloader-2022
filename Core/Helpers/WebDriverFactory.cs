using FacebookVideosDownloader.Core.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;

namespace FacebookVideosDownloader.Core.Helpers
{
    public class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver(Browser browser, bool headless = true)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "webdrivers");

            switch (browser)
            {
                case Browser.Firefox:
                    {
                        var options = new FirefoxOptions();

                        if (headless)
                            options.AddArgument("--headless");

                        return new FirefoxDriver(path, options);
                    }
                case Browser.Chrome:
                    {
                        var options = new ChromeOptions();

                        if (headless)
                            options.AddArgument("--headless");

                        return new ChromeDriver(path, options);
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
