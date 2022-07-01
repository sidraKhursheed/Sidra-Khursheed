using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Support.UI;

namespace Ios_Android_Project.PageObjectModel
{
    public class BasePageFindElement
    {
        //public IOSDriver<IOSElement> driverios;

        AppiumDriver<AppiumWebElement> basemobiledriver;

        public BasePageFindElement(AppiumDriver<AppiumWebElement> Driver)
        {
            basemobiledriver = Driver;

        }

        public string contextSwitching()
        {
            List<string> AllContexts = new List<string>();
            foreach (var context in (basemobiledriver.Contexts))
            {
                AllContexts.Add(context);
            }
            basemobiledriver.Context = (AllContexts[1]);
            // Switch to NATIVE_APP
            // driver.Context = ("NATIVE_APP");
            Thread.Sleep(1000);
            var PS = basemobiledriver.PageSource;
            return PS;
        }

        public IWebElement ElementByCss(String KeyName)
        {
            Console.WriteLine("Element Found " + KeyName);
            WaitForElement(KeyName, "CssSelector");
            return basemobiledriver.FindElement(By.CssSelector(KeyName));
        }

        private void WaitForElement(String Element, string Locator)
        {
            var wait = new DefaultWait<AppiumDriver<AppiumWebElement>>(basemobiledriver)
            {
                Timeout = TimeSpan.FromSeconds(120),
                PollingInterval = TimeSpan.FromMilliseconds(9000)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            if (Locator == "CssSelector")
                wait.Until(d => d.FindElement(MobileBy.CssSelector(Element)));
            else
                wait.Until(d => d.FindElement(MobileBy.XPath(Element)));

        }

        public IWebElement ElementByXpath(String KeyName)
        {
            Console.WriteLine("Element Found " + KeyName);
            WaitForElement(KeyName, "Xpath");
            return basemobiledriver.FindElement(By.XPath(KeyName));
        }

        public IWebElement ElementbyId(String KeyName)
        {
            Console.WriteLine("Element Found " + KeyName);
           
            return basemobiledriver.FindElement(By.Id(KeyName));
        }
    }
}
