using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace III_ProjectOne.Class
{
    internal class Dropdown
    {
        public static void Select(IWebDriver webDriver,string xpathValue,string option){
            try {
                IWebElement Displayrow_Dropdown = webDriver.FindElement(By.XPath(xpathValue));
                SelectElement oSelect_1 = new SelectElement(Displayrow_Dropdown);

                 oSelect_1.SelectByText(option);
                //--------------------------------------------------------------------------------------
                Thread.Sleep(3000);
                
            }
            catch(Exception  ex) {

                GlobalVariable.errorStatus = true;
              LogMessage.Log(ex.Message);
            }
        }

    }
}
