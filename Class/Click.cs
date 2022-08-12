using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace III_ProjectOne.Class
{
    internal class Click
    {
        public static bool ButtonClick(IWebDriver webDriver, string xpathValue,string waitElement)
        {
            try
            {
                string windowsHandle = webDriver.CurrentWindowHandle;
                //Click on the button and wait until secified element appears
                LogMessage.Log("Clicking button " + xpathValue);
                webDriver.FindElement(By.XPath(xpathValue)).Click();
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.XPath(waitElement)));
                windowsHandle = webDriver.CurrentWindowHandle;
                return true;
            }
            catch(Exception ex)
            {
                LogMessage.Log("Error: Click->ButtonCLick - " + ex.Message);
                LogMessage.Log("Error: Click->ButtonCLick - " + ex.StackTrace);
                return false;
            }
        }
    }
}
