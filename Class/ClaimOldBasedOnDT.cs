using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace III_ProjectOne.Class
{
    internal class ClaimOldBasedOnDT
    {
        public static bool ProcessClaim(IWebDriver webDriver, Label label)
        {
             string type = "Claim";
            string CurrentPolicyNo = null;
            string claimNo = null;
            try
            {
                //bool filterFlag = false;
                DataTable tblFiltered;
                string currentWindow = webDriver.CurrentWindowHandle;
                var currentWebDriver = webDriver;
                //Click on new notice of loss
                //webDriver.FindElement(By.X
                //
                //(GlobalVariable.navigationDict["NewNoticeOfLoss"])).Click();

                if (GlobalVariable.dtClaimData.Rows.Count > 0)
                {
                    
                        foreach (DataRow row in GlobalVariable.dtClaimData.Rows)
                        {
                        try
                        {
                            claimNo = null;
                            GlobalVariable.cancellationToken.ThrowIfCancellationRequested();
                            SwitchTab.SwitchToTab(currentWebDriver, GlobalVariable.navigationDict["NewNoticeOfLoss"], GlobalVariable.navigationDict["PolicyNoSearchIcon"], currentWindow, currentWebDriver.WindowHandles);
                            // filterFlag = false;
                            LogMessage.Log("-----------------------------------------------------");
                            LogMessage.Log("Processing Claim for the policy number " + row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim());
                            CurrentPolicyNo = row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim();
                            LabelText.UpdateText(label, "Processing Claim for the policy number: " + row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim());

                            try
                            {
                                LogMessage.Log("Filtering sheet 2 for policy number");
                                //filter second dt for the register number if found tblfilter will contain 1 row
                                tblFiltered = GlobalVariable.dtCustomerAgentData.AsEnumerable()
                                            .Where(r => r.Field<string>(GlobalVariable.mappingDict["PolicyNumberFilter"].ToString().Trim()) == row[GlobalVariable.mappingDict["PolicyNumberFilter"]].ToString().Trim())
                                            .CopyToDataTable();
                            }
                            catch (Exception ex)
                            {
                                LogMessage.Log("Error: Encountered issue while  searching for customer: " + row[GlobalVariable.mappingDict["PolicyNumberFilter"]] + " in pst file.");
                                LogMessage.Log("Error: " + ex.Message);
                                LogMessage.Log("Error: " + ex.StackTrace);
                                //filterFlag = true;
                                //GlobalVariable.AddDataToSummaryTabe(name, type, "Unable to find data", "FAIL");
                                continue;
                            }


                            //Click on Search icon
                            //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySearchButton"])).Click();
                            //Thread.Sleep(5000);
                            LogMessage.Log("Clicking on Search icon - PolicyNoSearchIcon");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyNoSearchIcon"], GlobalVariable.navigationDict["PolicySearchButton"]);

                            //Enter policy number
                            LogMessage.Log("Enetr Policy Number - PolicyNumber");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNumber"])).SendKeys(row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim());

                            //Click on search button
                            LogMessage.Log("Clicking on search button - PolicySearchButton");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySearchButton"])).Click();

                            Thread.Sleep(3000);

                            LogMessage.Log("Searcing the result");
                            string resultText = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyListTable"])).Text;
                            if (resultText.Contains(row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim()))
                            {
                                //Select the first policy from search
                                LogMessage.Log("Seleting first poicy from the search - PolicyNoRadioButton");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNoRadioButton"])).Click();

                                // MessageBox.Show("Please verify the selected policy");


                                //Press Coninue button
                                LogMessage.Log("Pressing Continue button - PolicyListContinueButton");
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyListContinueButton"], GlobalVariable.navigationDict["PolicyDateOfLoss"]);

                                //Filling date of loss
                                LogMessage.Log("Filling Date of loss field - PolicyDateOfLoss");
                                string tempVar = ConvertStringToDate.convertToDate(tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyDateOfLoss"]].ToString().Trim());
                                tempVar = DateTime.Parse(tempVar).ToString("dd/MM/yyyy");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfLoss"])).SendKeys(tempVar);

                                //Filling Date of notification
                                LogMessage.Log("Filling date of notice field - PolicyDateOfNotification");
                                tempVar = ConvertStringToDate.convertToDate(tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyDateOfNotification"]].ToString().Trim());
                                tempVar = DateTime.Parse(tempVar).ToString("dd/MM/yyyy");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfNotification"])).SendKeys(tempVar);

                                //Selecting the business type 
                                LogMessage.Log("Selecting the business type");
                                string policyType = row[GlobalVariable.mappingDict["PolicyNumber"]].ToString().Trim();

                                //extractng policy type from policy number
                                policyType = policyType.Substring(3, 3);
                                switch (policyType)
                                {
                                    case "FFR":
                                        //Selecting the Direct business
                                        LogMessage.Log("Seleting Direct business - PolicyDirectBusiness");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDirectBusiness"])).Click();
                                        break;

                                    case "IAR":
                                        //Selecting Reinsurance inward
                                        LogMessage.Log("Seleting  Reinsurance inward - PolicyReInsuranceInwards");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyReInsuranceInwards"])).Click();
                                        break;

                                    default:

                                        LogMessage.Log("Unable to detect the business type, defaulting to Reinsurance inward - PolicyReInsuranceInwards");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyReInsuranceInwards"])).Click();
                                        break;
                                }

                                //Selecting SIF/OIF
                                LogMessage.Log("Selecting the SIF/OIF");

                                if (row[GlobalVariable.mappingDict["PolicySIF_OIF"]].ToString().Trim().ToLower() == "oif")
                                {
                                    LogMessage.Log("Selecting OIF - PolicyOIF");
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyOIF"])).Click();
                                }
                                else
                                {
                                    LogMessage.Log("Selecting OIF - PolicySIF");
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySIF"])).Click();
                                }

                                //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfNotification"])).SendKeys(row[GlobalVariable.mappingDict["PolicyDateOfNotification"]].ToString().Trim());

                                //Click on retrieve button
                                LogMessage.Log("Click on retrieve button -PolicyRetrieve");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyRetrieve"])).Click();

                                //Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyRetrieve"], GlobalVariable.navigationDict["PolicyContinue"]);

                                Thread.Sleep(9000);
                                if (webDriver.FindElements(By.XPath(GlobalVariable.navigationDict["ClaimExistsPopup"])).Count != 0)
                                {
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimExistsPopup"])).Click();

                                }

                                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(90));
                                wait.Until(ExpectedConditions.ElementExists(By.XPath(GlobalVariable.navigationDict["PolicyContinue"])));


                                LogMessage.Log("Click on continue button");
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyContinue"], GlobalVariable.navigationDict["PolicyAddDamageInfo"]);
                                Thread.Sleep(3000);

                                //Fill Accident description
                                LogMessage.Log("Filling accident description. - PolicyAccidentDesc");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyAccidentDesc"])).SendKeys(row[GlobalVariable.mappingDict["PolicyAccidentDesc"]].ToString().Trim());

                                //Adding damage info
                                LogMessage.Log("Adding damage info -PolicyAddDamageInfo");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyAddDamageInfo"])).Click();

                                //selecting the party type
                                LogMessage.Log("Selecting the party type(defaulting to first party) -PolicyDamageFirstParty");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageFirstParty"])).Click();

                                //Drop down select
                                LogMessage.Log("Selecting Liabality as Damage type");
                                Dropdown.Select(webDriver, GlobalVariable.navigationDict["PolicyDamageType"], "Liability");

                                LogMessage.Log("Selecting Claimant");
                                IWebElement l = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PoicyDamageClaimant"]));
                                //object of SelectElement
                                SelectElement s = new SelectElement(l);
                                //Options method to get all options
                                IList<IWebElement> els = s.Options;
                                //count options
                                int e = els.Count;
                                for (int j = 0; j < e; j++)
                                {
                                    if ((els.ElementAt(j).Text).ToString().Contains(row[GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim()))
                                    {
                                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], els.ElementAt(j).Text);
                                        break;
                                    }

                                }


                                //Selecting claimant

                                //Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], row[GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim());

                                //Clicking on save

                                //MessageBox.Show("Please verify the policy and damage information then press OK");
                                LogMessage.Log("Clicking on save - PolicyDamageSave");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageSave"])).Click();

                                //Clicking on the save
                                LogMessage.Log("Clicking on policy save - PolicySave");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySave"])).Click();

                                //Clicking on submit
                                LogMessage.Log("Clicking on policy submit - PolicySubmit");
                                //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySubmit"])).Click();
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicySubmit"], GlobalVariable.navigationDict["ManualSubmit"]);

                                //Click on Manual Submit
                                LogMessage.Log("Clicking submit on popup");
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["ManualSubmit"], GlobalVariable.navigationDict["WorkOn"]);

                                LogMessage.Log("Selecting the claim - WorkOnClaimSelect");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["WorkOnClaimSelect"])).Click();

                                LogMessage.Log("Clicking on WorkOn button");
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["WorkOn"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                                LogMessage.Log("Capturing claim number");
                                claimNo = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["GetClaimNumber"])).Text;
                                LogMessage.Log("Claim number is" + claimNo);

                                string policyNo = row[GlobalVariable.mappingDict["DefaultClaimNumber"]].ToString().Trim();
                                policyNo = "PSTMIG - " + policyNo.ToString().Trim();

                                //Filling Refnumber
                                LogMessage.Log("Filling reference Number: " + policyNo);
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ReferenceNo"])).SendKeys(policyNo);

                                //Selecting Cause of loss 
                                LogMessage.Log("Selcting cause of loss CauseofLossDropdown");
                                Dropdown.Select(webDriver, GlobalVariable.navigationDict["CauseofLossDropdown"], GlobalVariable.defaultValues["CauseofLossDropdown"].ToString().Trim());

                                //Selecting subclaim insured object
                                LogMessage.Log("Selecting subclim insured object");
                                GlobalVariable.errorStatus = false;
                                Dropdown.Select(webDriver, GlobalVariable.navigationDict["SubClaimInsuredObject"], row[GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim());
                                if (GlobalVariable.errorStatus)
                                {

                                    GlobalVariable.errorStatus = false;
                                    l = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimInsuredObject"]));
                                    //object of SelectElement
                                    s = new SelectElement(l);
                                    //Options method to get all options
                                    els = s.Options;
                                    //count options
                                    e = els.Count;
                                    for (int j = 0; j < e; j++)
                                    {
                                        if ((els.ElementAt(j).Text).ToString() != "Please Select")
                                        {
                                            Dropdown.Select(webDriver, GlobalVariable.navigationDict["SubClaimInsuredObject"], els.ElementAt(j).Text);
                                            break;
                                        }

                                    }
                                }

                                //Selecting subclaim type object
                                LogMessage.Log("Selecting SubClaimType");
                                GlobalVariable.errorStatus = false;
                                Dropdown.Select(webDriver, GlobalVariable.navigationDict["SubClaimType"], "Other Benefit");
                                if (GlobalVariable.errorStatus)
                                {

                                    GlobalVariable.errorStatus = false;
                                    l = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimType"]));
                                    //object of SelectElement
                                    s = new SelectElement(l);
                                    //Options method to get all options
                                    els = s.Options;
                                    //count options
                                    e = els.Count;
                                    for (int j = 0; j < e; j++)
                                    {
                                        if ((els.ElementAt(j).Text).ToString() != "Please Select")
                                        {
                                            Dropdown.Select(webDriver, GlobalVariable.navigationDict["SubClaimType"], els.ElementAt(j).Text);
                                            break;
                                        }

                                    }
                                }




                                //Selecting subclaimClaimOwner 
                                GlobalVariable.errorStatus = false;
                                LogMessage.Log("Selecting subclaimClaimOwner");
                                Dropdown.Select(webDriver, GlobalVariable.navigationDict["subclaimClaimOwner"], GlobalVariable.configDict["Username"].ToString().Trim());
                                if (GlobalVariable.errorStatus)
                                {
                                    GlobalVariable.errorStatus = false;
                                    LogMessage.Log("Unable to find the Subclaim owner, " + GlobalVariable.configDict["Username"].ToString().Trim());
                                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["subclaimClaimOwner"], GlobalVariable.defaultValues["DefaultUsernameForSuClaimOwner"].ToString().Trim());
                                }

                                //Select subClaimCoverageSelect
                                LogMessage.Log("Selecting the subClaimCoverageSelect");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["subClaimCoverageSelect"])).Click();


                                //  MessageBox.Show("Pease verify the info then press OK");
                                //Save buttom ClaimSave
                                LogMessage.Log(" Clicking ClaimSave");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimSave"])).Click();

                                //submit button
                                LogMessage.Log(" Clicking ClaimSubmit");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimSubmit"])).Click();

                                // SubClaimManualAssignment
                                LogMessage.Log(" Clicking SubClaimManualAssignment");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimManualAssignment"])).Click();

                                //SubClaimTaskTransfer
                                LogMessage.Log(" Clicking SubClaimTaskTransfer");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimTaskTransfer"])).Click();


                                //BackToClaimTaskTransfer
                                LogMessage.Log(" Clicking BackToClaimTaskTransfer");
                                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["BackToClaimTaskTransfer"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                                //Entering the 

                                webDriver.Close();
                                webDriver.SwitchTo().Window(currentWindow);
                                GlobalVariable.AddDataToSummaryTabe(CurrentPolicyNo, claimNo, "", "PASS", "Claim");
                                // return true;
                            }
                            else
                            {
                                LogMessage.Log("Policy not found.");
                                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNumber"])).Clear();
                                GlobalVariable.AddDataToSummaryTabe(CurrentPolicyNo, claimNo, "Policy Number not avialable ", "PASS", "Claim");
                                continue;
                            }
                        }

                        catch (Exception ex)
                        {
                            GlobalVariable.AddDataToSummaryTabe(CurrentPolicyNo, claimNo, ex.Message, "FAIL", "Claim");
                           
                            LogMessage.Log("Error: Claim -> ProcessClaim -" + ex.Message);
                            LogMessage.Log("Error: Claim -> ProcessClaim -" + ex.StackTrace);
                            webDriver.Close();
                            webDriver.SwitchTo().Window(currentWindow);
                            //Check for login page
                           

                        }



                }
                    
                   
             }

                return true;
            }
            catch (Exception ex)
            {
                //GlobalVariable.AddDataToSummaryTabe(CurrentPolicyNo, claimNo, ex.Message, "FAIL", "Claim");
                LogMessage.Log("Error: Claim -> ProcessClaim -" + ex.Message);
                LogMessage.Log("Error: Claim -> ProcessClaim -" + ex.StackTrace);
                return false;
            }
        }
    }
}
