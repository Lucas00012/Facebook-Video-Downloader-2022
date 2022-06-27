using FacebookVideosDownloader.Core.Enums;
using FacebookVideosDownloader.Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookVideosDownloader.Core.Entities
{
    public class SeleniumScraperEngine : IDisposable
    {
        private CustomWebDriverWait Wait { get; set; }
        private IWebDriver WebDriver { get; set; }

        public SeleniumScraperEngine(Browser browser, TimeSpan timeSpan, bool headless = true)
        {
            WebDriver = WebDriverFactory.CreateWebDriver(browser, headless);
            Wait = new CustomWebDriverWait(WebDriver, timeSpan);

            WebDriver.Manage().Window.Maximize();
            Wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(NotFoundException));
        }

        public SeleniumScraperEngine(Browser browser, bool headless = true) : this(browser, TimeSpan.FromSeconds(30), headless)
        {

        }

        public void Navigate(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
        }

        public IWebElement GetElement(By by)
        {
            return Wait.Until(d => d.FindElement(by));
        }

        public IWebElement GetElementByCssSelector(string selector)
        {
            return Wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        public List<IWebElement> GetElements(By by)
        {
            return Wait.Until(d => d.FindElements(by))
                .ToList();
        }

        public IWebElement GetElementById(string id)
        {
            return Wait.Until(d => d.FindElement(By.Id(id)));
        }

        public IWebElement GetElementByXPath(string xpath)
        {
            return Wait.Until(d => d.FindElement(By.XPath(xpath)));
        }

        public List<IWebElement> GetElementsByXPath(string xpath)
        {
            return Wait.Until(d => d.FindElements(By.XPath(xpath)))
                .ToList();
        }

        public IWebElement GetElementByClassName(string className)
        {
            return Wait.Until(d => d.FindElement(By.ClassName(className)));
        }

        public IWebElement GetElementByLinkText(string linkText)
        {
            return Wait.Until(d => d.FindElement(By.LinkText(linkText)));
        }

        public List<IWebElement> GetElementsByClassName(string className)
        {
            return Wait.Until(d => d.FindElements(By.ClassName(className)))
                .ToList();
        }

        public IWebElement GetElementByTagName(string tag)
        {
            return Wait.Until(d => d.FindElement(By.TagName(tag)));
        }

        public IWebElement GetButtonByType(string type)
        {
            var elements = Wait.Until(d => d.FindElements(By.CssSelector("button")));

            return elements.FirstOrDefault(e => e.GetAttribute("type") == type);
        }

        public void NavigateBack(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                WebDriver.Navigate().Back();
            }
        }

        public string GetCurrentUrl()
        {
            return WebDriver.Url;
        }

        public bool UrlEquals(string url)
        {
            return Wait.Until(d => GetCurrentUrl().TrimEnd('/') == url.TrimEnd('/'));
        }

        public bool UrlContains(string url)
        {
            return Wait.Until(d => GetCurrentUrl().Contains(url));
        }

        public bool ExistsElement(By by)
        {
            try
            {
                WebDriver.FindElement(by);

                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void Close()
        {
            WebDriver.Close();
        }

        public void Dispose()
        {
            WebDriver.Quit();
        }
    }
}
