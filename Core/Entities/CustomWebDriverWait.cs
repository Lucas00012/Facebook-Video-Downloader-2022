using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace FacebookVideosDownloader.Core.Entities
{
    public class CustomWebDriverWait : IWait<IWebDriver>
    {
        private static readonly TimeSpan _defaultSleepTimeout = TimeSpan.FromMilliseconds(500.0);

        public TimeSpan Timeout { get; set; }
        public TimeSpan PollingInterval { get; set; }
        public string Message { get; set; }

        private IClock Clock { get; set; }
        private IWebDriver WebDriver { get; set; }
        private List<Type> IgnoredExceptions { get; set; }

        public CustomWebDriverWait(IWebDriver webDriver, TimeSpan timeout, TimeSpan pollingInterval)
        {
            IgnoredExceptions = new List<Type>();
            PollingInterval = pollingInterval;
            Clock = new SystemClock();

            WebDriver = webDriver;
            Timeout = timeout;

            IgnoreExceptionTypes(typeof(NotFoundException));
        }

        public CustomWebDriverWait(IWebDriver webDriver, TimeSpan timeout) : this(webDriver, timeout, _defaultSleepTimeout)
        {

        }

        public void IgnoreExceptionTypes(params Type[] exceptionTypes)
        {
            if (exceptionTypes == null)
                throw new ArgumentNullException("exceptionTypes", "exceptionTypes cannot be null");

            if (exceptionTypes.Any(e => !e.IsSubclassOf(typeof(Exception))))
                throw new ArgumentException("All types to be ignored must derive from System.Exception", "exceptionTypes");

            IgnoredExceptions.AddRange(exceptionTypes);
        }

        public TResult Until<TResult>(Func<IWebDriver, TResult> condition)
        {
            return Until(condition, CancellationToken.None);
        }

        public TResult Until<TResult>(Func<IWebDriver, TResult> condition, CancellationToken token)
        {
            if (condition == null)
                throw new ArgumentNullException("condition", "condition cannot be null");

            var typeFromHandle = typeof(TResult);
            if (typeFromHandle.IsValueType && typeFromHandle != typeof(bool) || !typeof(object).IsAssignableFrom(typeFromHandle))
                throw new ArgumentException("Can only wait on an object or boolean response, tried to use type: " + typeFromHandle.ToString(), "condition");

            Exception lastException = null;

            var limitDateTime = Clock.LaterBy(Timeout);

            while (true)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    var result = condition(WebDriver);

                    if (result is bool boolean && boolean)
                        return result;
                    else if (result != null)
                        return result;
                }
                catch (Exception ex)
                {
                    if (!IsIgnoredException(ex))
                        throw;

                    lastException = ex;
                }

                if (!Clock.IsNowBefore(limitDateTime))
                {
                    var text = string.Format(CultureInfo.InvariantCulture, "Timed out after {0} seconds", Timeout.TotalSeconds);
                    if (!string.IsNullOrEmpty(Message))
                        text = $"{text} : {Message}";

                    throw new WebDriverTimeoutException(text, lastException);
                }

                Thread.Sleep(Timeout);
            }
        }

        private bool IsIgnoredException(Exception exception)
        {
            return IgnoredExceptions.Contains(exception.GetType());
        }
    }
}
