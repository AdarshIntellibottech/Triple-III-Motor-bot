using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace III_ProjectOne.Class
{
    class PortalLogin
    {
        public static bool LoginToPortal(IWebDriver webDriver)
        {
            int sleepCtr = 1000;
            string tempString = null;
            string tempXpath = null;

            try
            {

                tempString = GlobalVariable.configDict["Url"];
                webDriver.Url = tempString;
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(180));

                //wait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div[1]/div[2]/div/div/div[2]/ul/li[2]/a")));
              
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[accesskey='l']")));

                //Enter User name
                // webDriver.FindElement(By.Name("[accesskey='u']")).SendKeys("javatpoint tutorials");
                tempString = GlobalVariable.configDict["Username"];
                tempXpath = GlobalVariable.LoginNavigation["Username"];
                webDriver.FindElement(By.XPath(tempXpath)).SendKeys(tempString);
                //webDriver.FindElement(By.CssSelector("[accesskey='u']")).SendKeys(tempString);
                Thread.Sleep(sleepCtr);

                //Entering Password
                tempString = GlobalVariable.configDict["Password"];
                tempXpath = GlobalVariable.LoginNavigation["Password"];
                //webDriver.FindElement(By.CssSelector("[accesskey='p']")).SendKeys(tempString);
                webDriver.FindElement(By.XPath(tempXpath)).SendKeys(tempString);
                Thread.Sleep(sleepCtr);

                //Click Login
                tempXpath = GlobalVariable.LoginNavigation["LogIn"];
                tempString = GlobalVariable.LoginNavigation["PolicyAdministration"];
                var resultFlag = Click.ButtonClick(webDriver,tempXpath, tempString);
                //webDriver.FindElement(By.CssSelector("[accesskey='l']")).Click();

                if (resultFlag)
                {
                    LogMessage.Log("Login successful...");
                    return true;
                }
                else
                {
                    LogMessage.Log("Login Failed...");
                    return false;
                }

                //Checking for invalid credentials message
                //var source = webDriver.FindElements(By.Id("msg"));
                
            }
            catch(Exception ex)
            {
                LogMessage.Log("Error: LoginToPortal -"+ex.Message);
                LogMessage.Log("Error: LoginToPortal -"+ex.StackTrace);
                return false;

            }
           
            
        }
    }
}
