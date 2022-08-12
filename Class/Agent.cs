using OpenQA.Selenium;
using System;
using System.Data;
using System.Threading;

namespace III_ProjectOne.Class
{
    internal class Agent
    {
        public static string tempWaitElement = null;
        public static string tempXpath = null;
        public static string type = "Agent";

        internal static bool ProcessAgentData(IWebDriver browserInstace)
        {
            bool result;


            try {

                //Select Sales Channel maintainace from SalesChannel Dropdown

                browserInstace.FindElement(By.XPath(GlobalVariable.navigationDict["SalesChannelDropdown"])).Click();

                browserInstace.FindElement(By.XPath(GlobalVariable.navigationDict["SalesChannelMaintainance"])).Click();

                Thread.Sleep(3000);

                foreach (DataRow row in GlobalVariable.dtClaimData.Rows)
                {
                    LogMessage.Log("---------------------------------------------");
                    GlobalVariable.errorStatus = false;


                    result = SearchAgent(browserInstace, row);

                    if (!result && !GlobalVariable.errorStatus)
                    {
                        result = CreateAgent(browserInstace, row);
                    }

                }


                Thread.Sleep(1000);


                LogMessage.Log("Done.");


                return true;
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error: Agent > Process AgentData -" + ex.Message);
                LogMessage.Log("Error: Agent > Process AgentData -" + ex.StackTrace);
                return false;
            }
        }

        private static bool CreateAgent(IWebDriver webDriver, DataRow row)
        {
            try {

                return true;
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error: Agent > CreateAgent -" + ex.Message);
                LogMessage.Log("Error: Agent > CreateAgent -" + ex.StackTrace);
                return false;

            }
        }

        private static bool SearchAgent(IWebDriver webDriver, DataRow row)
        {
            


                string name = row[GlobalVariable.mappingDict["AgentName"]].ToString().Trim();
                //string tempXpath;
                try
                {

                    LogMessage.Log("Searching Agent " + name);

                    //Select dropdown
                    //tempXpath = GlobalVariable.navigationDict["CustomerTypeDropdown"];

                   // Dropdown.Select(webDriver, tempXpath, row[GlobalVariable.mappingDict["CustomerType"]].ToString().Trim());

                    //enter customer name
                    tempXpath = GlobalVariable.navigationDict["SalesChannelAgentName"];
                    webDriver.FindElement(By.XPath(tempXpath)).Clear();
                    webDriver.FindElement(By.XPath(tempXpath)).SendKeys(name);

                    //Click on search
                    tempXpath = GlobalVariable.navigationDict["SalesChannelAgentSearch"];
                    webDriver.FindElement(By.XPath(tempXpath)).Click();

                    Thread.Sleep(5000);

                    //Check Table value
                    tempXpath = GlobalVariable.navigationDict["SalesChannelAgentSearchResult"];
                    string resultText = webDriver.FindElement(By.XPath(tempXpath)).Text;
                    if (resultText.Contains(name))
                    {
                        LogMessage.Log("Agent data found, skipping creation.");
                        GlobalVariable.AddDataToSummaryTabe(name, type, "Agent data found, skipping creation", "PASS",type);
                        return true;
                    }
                    else
                    {
                        return false;
                    }




                }
                catch (Exception ex)
                {
                GlobalVariable.AddDataToSummaryTabe(name, type, "Error while searching for the Agent", "UNKNOWN",type);
                LogMessage.Log("Error in searching for Agent -" + name);
                LogMessage.Log("Error: Agent->SearchAgent - " + ex.Message);
                LogMessage.Log("Error: Agent->SearchAgent - " + ex.StackTrace);
                GlobalVariable.errorStatus = true;
                return false;
                

                }
        }
    }   
}