using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace III_ProjectOne.Class
{
    class ProcessData
    {
        public void startProcessing(Label label, string optionSelected,CheckBox checkBox)
        {
            bool result;
            try
            {
                LabelText.UpdateText(label, "Starting chromium browser.");

                var browserInstace = startChromiumBrowser.Start();
                if (browserInstace != null)
                {
                    result=  PortalLogin.LoginToPortal(browserInstace);
                    if (!result) 
                    {
                        LogMessage.Log("Error - Login failed, check credentials....");
                        browserInstace.Close();
                        browserInstace.Quit();
                        browserInstace.Dispose();
                        throw new Exception();
                    }

                    string currentWindow = browserInstace.CurrentWindowHandle;
                    string tempXpath = null;
                    string tempWaitElement = null;
                    switch (optionSelected)
                    {
                        case "Customer":
                            tempXpath = GlobalVariable.navigationDict["PolicyAdministration"];
                            tempWaitElement = GlobalVariable.navigationDict["Customer"];
                            result = SwitchTab.SwitchToTab(browserInstace, tempXpath, tempWaitElement, currentWindow, browserInstace.WindowHandles);
                            break;

                        case "Agent":
                            tempXpath = GlobalVariable.navigationDict["SalesChannel"];
                            tempWaitElement = GlobalVariable.navigationDict["SalesChannelDropdown"];
                            result = SwitchTab.SwitchToTab(browserInstace, tempXpath, tempWaitElement, currentWindow,browserInstace.WindowHandles);
                            break;

                        case "Claim":
                            tempXpath = GlobalVariable.navigationDict["Claim"];
                            tempWaitElement = GlobalVariable.navigationDict["NewNoticeOfLoss"];
                            result = SwitchTab.SwitchToTab(browserInstace, tempXpath, tempWaitElement, currentWindow,browserInstace.WindowHandles);
                            if (!result)
                            {
                                LogMessage.Log("Error - Navigating to " + optionSelected + " failed!.");
                                //browserInstace.Close();
                                browserInstace.Quit();
                                browserInstace.Dispose();
                              //  throw new Exception();
                            }
                            //result = Claim.ProcessClaim(browserInstace,label);
                            result = ClaimProcess.ProcessClaim(browserInstace,label,checkBox);

                            break;


                    }
                    

                    


                    browserInstace.Quit();
                    browserInstace.Dispose();
                    
                  

                }
                else
                {
                    LogMessage.Log("Unable to start chromium browser");
                    LabelText.UpdateText(label, "Unable to start chromium browser");
                    MessageBox.Show("Unable to start browser" + Environment.NewLine + "Check log for more info.");
                    Application.Exit();
                }

            }
            
            catch(Exception ex)
            {
                MessageBox.Show("Error occured,check log file for more information.");
                LogMessage.Log("Error: satrtProcessing - "+ex.Message);
                LogMessage.Log("Error: satrtProcessing - "+ex.StackTrace);
                Application.Exit();
            }

        }
    }
}
