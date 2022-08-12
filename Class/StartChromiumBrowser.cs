using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace III_ProjectOne.Class
{
    class startChromiumBrowser
    {
        public static IWebDriver Start()
        {
            //Code to  start chrmium browser
            //IWebDriver m_driver =null;
            try
            {
                LogMessage.Log("Starting chromium browser.");
                //ChromeDriverService service = ChromeDriverService.CreateDefaultService(GlobalVariable.configDict["Chromium driver path"]);
                ChromeDriverService service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
                service.HideCommandPromptWindow = true;

                var chrome_options = new ChromeOptions();
                chrome_options.BinaryLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chromium\\Application\\chrome.exe";
               
                GlobalVariable.m_driver = new ChromeDriver(service, chrome_options, TimeSpan.FromSeconds(180));

                GlobalVariable.m_driver.Manage().Window.Maximize();
               
                
                return GlobalVariable.m_driver;

            }
            catch(Exception ex)
            {
                LogMessage.Log("Error: startChromiumBrowser - " + ex.Message);
                LogMessage.Log("Error: startChromiumBrowser - " + ex.StackTrace);
                if (GlobalVariable.m_driver == null)
                {
                }
                else
                {
                    GlobalVariable.m_driver.Quit();
                }
                return GlobalVariable.m_driver = null;
                
            }

            

           
        } 
    }
}
