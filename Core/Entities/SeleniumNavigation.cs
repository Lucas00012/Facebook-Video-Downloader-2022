using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookVideosDownloader.Core.Entities
{
    public class SeleniumNavigation
    {
        private const int TIMEOUT = 30;

        public SeleniumNavigation(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            _wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(TIMEOUT));

            _webDriver.Manage().Window.Maximize();
        }

        private readonly WebDriverWait _wait;
        private readonly IWebDriver _webDriver;

        public void Navigate(string url)
        {
            _webDriver.Navigate().GoToUrl(url);
        }

        public IWebElement GetElement(By by)
        {
            return _wait.Until(d => d.FindElement(by));
        }

        public List<IWebElement> GetElements(By by)
        {
            return _wait.Until(d => d.FindElements(by))
                .ToList();
        }

        public IWebElement GetElementById(string id)
        {
            return _wait.Until(d => d.FindElement(By.Id(id)));
        }

        public IWebElement GetElementByXPath(string xpath)
        {
            return _wait.Until(d => d.FindElement(By.XPath(xpath)));
        }

        public List<IWebElement> GetElementsByXPath(string xpath)
        {
            return _wait.Until(d => d.FindElements(By.XPath(xpath)))
                .ToList();
        }

        public IWebElement GetElementByClassName(string className)
        {
            return _wait.Until(d => d.FindElement(By.ClassName(className)));
        }

        public IWebElement GetElementByLinkText(string linkText)
        {
            return _wait.Until(d => d.FindElement(By.LinkText(linkText)));
        }

        public List<IWebElement> GetElementsByClassName(string className)
        {
            return _wait.Until(d => d.FindElements(By.ClassName(className)))
                .ToList();
        }

        public IWebElement GetElementByTagName(string tag)
        {
            return _wait.Until(d => d.FindElement(By.TagName(tag)));
        }

        public void NavigateBack(int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                _webDriver.Navigate().Back();
            }
        }

        public string GetCurrentUrl()
        {
            return _webDriver.Url;
        }

        public bool UrlEquals(string url)
        {
            return _wait.Until(d => GetCurrentUrl().TrimEnd('/') == url.TrimEnd('/'));
        }

        public bool UrlContains(string url)
        {
            return _wait.Until(d => GetCurrentUrl().Contains(url));
        }

        public bool ExistsElement(By by)
        {
            try
            {
                _webDriver.FindElement(by);

                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
