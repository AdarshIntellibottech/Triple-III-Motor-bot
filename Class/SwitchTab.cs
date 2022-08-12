using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace III_ProjectOne.Class
{
    internal class SwitchTab
    {
        public static bool SwitchToTab(IWebDriver webDriver, string xpathValue, string waitElement, string originalWindowHandle, ReadOnlyCollection<String> TabDetails)
        {
            try {

                var tabNos = webDriver.WindowHandles;
                LogMessage.Log("Clicking button " + xpathValue);
                webDriver.FindElement(By.XPath(xpathValue)).Click();

                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(180));
                 wait.Until(wd => wd.WindowHandles.Count == TabDetails.Count+1);
                

                // webDriver.SwitchTo().Window("");

                 foreach (string window in webDriver.WindowHandles)
                 {
                    /* if (originalWindowHandle != window)
                     {
                         webDriver.SwitchTo().Window(window);
                         break;
                     }*/

                    if (!(TabDetails.Contains (window)))
                    {
                        webDriver.SwitchTo().Window(window);
                        break;
                    }

                }
               // webDriver.SwitchTo().Window("Policy Administration");
                //Wait for the new tab to finish loading content
                //wait.Until(wd => wd.Title == "Policy Administration");

              

                wait.Until(ExpectedConditions.ElementExists(By.XPath(waitElement)));
                //string windowsHandle = webDriver.CurrentWindowHandle;
                return true;
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error: SwitchTab -> SwitchToTab - " + ex.Message);
                LogMessage.Log("Error: SwitchTab -> SwitchToTab - " + ex.StackTrace);
                return false;
            }
        }
    }
}
