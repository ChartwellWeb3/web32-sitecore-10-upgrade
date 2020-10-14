using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace SeleniumTests.Helpers
{
    public class FindElement
    {
        public WebDriverWait WebDriverWait;

        [Obsolete]
        public void Wait(IWebDriver webDriver, By by, int timeoutSeconds)
        {
            WebDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutSeconds));
            var element = WebDriverWait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        [Obsolete]
        public void WaitUntilElementExists(IWebDriver webDriver, By by, int timeoutSeconds)
        {
            WebDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutSeconds));
            var element = WebDriverWait.Until(ExpectedConditions.ElementExists(by));
        }

        [Obsolete]
        public IWebElement WebElement(IWebDriver webDriver, By by, int timeoutSeconds)
        {
            Wait(webDriver, by, timeoutSeconds);
            return webDriver.FindElement(by);
        }

        public IList<IWebElement> WebElements(IWebDriver webDriver, By by)
        {
            return webDriver.FindElements(by);
        }
    }
}
