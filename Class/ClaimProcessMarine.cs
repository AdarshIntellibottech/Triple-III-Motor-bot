using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace III_ProjectOne.Class
{
    internal class ClaimProcessMarine
    {
        static string ClaimNumber = null;
        public static bool ProcessClaim(IWebDriver webDriver, Label label,CheckBox checkBox)
        {
            var excelApp = new Excel.Application();
            Excel.Workbook excelWorkBook = null;
            LogMessage.Log("Started processing claim");
            string currentWindowHandle = webDriver.CurrentWindowHandle;
            

            try
            {
                excelWorkBook = excelApp.Workbooks.Open(GlobalVariable.configDict["ClaimDataInputFile"]);
               
                //Excel._Worksheet excelSheet = excelWorkBook.Sheets[1];  //workbook.Sheets["Sheet2"];
                Excel._Worksheet excelSheet = excelWorkBook.Sheets[GlobalVariable.configDict["ClaimDataInputFileSheetName"]];  //workbook.Sheets["Sheet2"];

                Excel.Range excelRange = excelSheet.UsedRange;
                int rows = excelRange.Rows.Count;
                

                //Get Column index of PolicyNumber., Claim Number# and PolicyNumber#,Status and Comments.
                int policyNumberDtIndex = ClaimProcess.GetColumnIndex(excelRange,excelSheet, GlobalVariable.mappingDict["PolicyNumberFilter"].ToString().Trim());
                int policyNumberIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["PolicyNumber"].ToString().Trim()); 
                int primaryClaimNumberIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["DefaultClaimNumber"].ToString().Trim()); 
                int ClaimNumberIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["ClaimNumber"].ToString().Trim());
                int statusIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["Status"].ToString().Trim());
                int commentIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["Comments"].ToString().Trim());
                int totalBaseAmtIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["BaseTotalOutStanding"].ToString().Trim());
                int causeofLossIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["CauseofLoss"].ToString().Trim());
                //int DRIIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["DRI"].ToString().Trim());
                int GadiNumberIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["GadiNumber"].ToString().Trim());
                //eBaoClass is not required for motor related insurance
                //int eBaoClassIndex = ClaimProcess.GetColumnIndex(excelRange, excelSheet, GlobalVariable.mappingDict["eBaoClass"].ToString().Trim());
               

                string policyNumberDt = null;
                string policyNumber = null;
                string claimNumber = null;
                string defaultClaimNumber = null;
                string totalBaseAmt = null;
                string causeofLoss = null;
                //string DRI = null;
                string eBaoClass = null;
                string gadiNumber = null;

                for (int rCnt = 2; rCnt <= excelRange.Rows.Count; rCnt++)
                {
                    GlobalVariable.cancellationToken.ThrowIfCancellationRequested();
                    policyNumberDt = null;
                    claimNumber = null;
                    policyNumber = null;
                    defaultClaimNumber = null;
                    totalBaseAmt = null;
                    ClaimNumber = null;
                    string CellVal = String.Empty;
                    try
                    {
                        policyNumber = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, policyNumberIndex]).Value2);
                        
                        defaultClaimNumber = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, primaryClaimNumberIndex]).Value2);
                        if((policyNumber != null)&&(defaultClaimNumber!=null))
                        {
                            claimNumber = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, ClaimNumberIndex]).Value2);
                            if (claimNumber == null)
                            {
                                policyNumberDt= Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, policyNumberDtIndex]).Value2);
                                if (policyNumberDt == null)
                                {
                                    excelSheet.Cells[rCnt, statusIndex] = "FAIL";
                                    excelSheet.Cells[rCnt, commentIndex] = "Old Policy number(col 3) not found.";
                                    LogMessage.Log("Policy number not found");
                                    continue;
                                }
                                else
                                {
                                    string claimNo = null;
                                    gadiNumber = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, GadiNumberIndex]).Value2);
                                    totalBaseAmt = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, totalBaseAmtIndex]).Value2);
                                    causeofLoss = Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, causeofLossIndex]).Value2);
                                    GlobalVariable.cancellationToken.ThrowIfCancellationRequested();
                                    SwitchTab.SwitchToTab(webDriver, GlobalVariable.navigationDict["NewNoticeOfLoss"], GlobalVariable.navigationDict["PolicyNoSearchIcon"], webDriver.CurrentWindowHandle, webDriver.WindowHandles);
                                    // filterFlag = false;
                                    LogMessage.Log("-----------------------------------------------------");
                                    LogMessage.Log("Processing Claim for the policy number " + policyNumber);
                                   
                                    LabelText.UpdateText(label, "Processing Claim for the policy number: " + policyNumber);
                                    DataTable tblFiltered = null;
                                    //DataTable tblVesselFiltered = null;
                                    try
                                    {
                                        LogMessage.Log("Filtering sheet 2 for policy number");
                                        //filter second dt for the register number if found tblfilter will contain 1 row
                                        tblFiltered = GlobalVariable.dtCustomerAgentData.AsEnumerable()
                                                    .Where(r => r.Field<string>(GlobalVariable.mappingDict["PolicyNumberFilter"].ToString().Trim()) == policyNumberDt.ToString().Trim() &&
                                                    r.Field<string>(GlobalVariable.mappingDict["DefaultClaimNumber"].ToString().Trim()) == defaultClaimNumber.ToString().Trim()
                                                    )
                                                    .CopyToDataTable();

                                        //filter the data in vessel data
                                        //tblVesselFiltered = GlobalVariable.dtVesselData.AsEnumerable()
                                        //            .Where(r => r.Field<string>(GlobalVariable.mappingDict["VesselPolicyNumber"].ToString().Trim()) == policyNumberDt.ToString().Trim() &&
                                        //            r.Field<string>(GlobalVariable.mappingDict["VesselClaimNumber"].ToString().Trim()) == defaultClaimNumber.ToString().Trim()
                                        //            )
                                        //            .CopyToDataTable();





                                    }
                                    catch (Exception ex)
                                    {
                                        LogMessage.Log("Error: Encountered issue while  searching for policy: " +policyNumberDt + " in settlement or vessel sheet.");
                                        LogMessage.Log("Error: " + ex.Message);
                                        LogMessage.Log("Error: " + ex.StackTrace);
                                        //filterFlag = true;
                                        //GlobalVariable.AddDataToSummaryTabe(name, type, "Unable to find data", "FAIL");
                                       
                                        excelSheet.Cells[rCnt, statusIndex] = "FAIL";
                                        excelSheet.Cells[rCnt, commentIndex] = "Data not found in the settlement sheet or vessel sheet";
                                        excelWorkBook.Save();
                                        continue;
                                    }
                                    string insType = null;
                                    //eBaoClass= Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, eBaoClassIndex]).Value2);
                                    //DRI= Convert.ToString(((Excel.Range)excelSheet.Cells[rCnt, DRIIndex]).Value2);
                                    //insType = DRI.ToString().Trim();
                                    //if (eBaoClass.ToString().Trim().ToLower() == "mmh")
                                    //{
                                    //    if (policyNumberDt.Substring(0, 2) == "H0")
                                    //    {
                                    //        insType = "CoIns";
                                    //    }
                                    //    else
                                    //    {
                                    //        insType = "RI Inward";
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //insType = DRI.ToString().Trim();
                                    //}
                                    Dictionary<string, string> outDict = new Dictionary<string, string>();

                                    outDict = Navigation(tblFiltered, webDriver, policyNumber, totalBaseAmt,gadiNumber,checkBox,insType,causeofLoss);

                                    //outDict["Result"] = "FAIL";
                                    //outDict["Comment"] = "Policy not found in eBAO system";
                                    excelSheet.Cells[rCnt, statusIndex] = outDict["Result"].ToString();
                                    excelSheet.Cells[rCnt, ClaimNumberIndex] = ClaimNumber;
                                    excelSheet.Cells[rCnt, commentIndex] = outDict["Comment"].ToString();
                                    excelWorkBook.Save();

                                    webDriver.Close();
                                    webDriver.SwitchTo().Window(currentWindowHandle);


                                }

                            }
                        }
                       
                        
                    }
                    catch (Exception ex)
                    {
                        LogMessage.Log("Error ClaimProcess->ProcessClaim :" + ex.Message);
                        LogMessage.Log("Error ClaimProcess->ProcessClaim :" + ex.StackTrace);
                        if (!(string.IsNullOrEmpty(ClaimNumber)))
                            excelSheet.Cells[rCnt, ClaimNumberIndex] = ClaimNumber;
                        excelSheet.Cells[rCnt, statusIndex] = "FAIL";
                        excelSheet.Cells[rCnt, commentIndex] = "Something went wrong, please check the log file.";
                        excelWorkBook.Save();
                        GlobalVariable.cancellationToken.ThrowIfCancellationRequested();
                        LogMessage.Log("Error ClaimProcess->ProcessClaim :Restarting browser");
                        webDriver.Quit();
                        webDriver = startChromiumBrowser.Start();
                        LogMessage.Log("Error ClaimProcess->ProcessClaim :Switching to Claim");
                        PortalLogin.LoginToPortal(webDriver);
                        SwitchTab.SwitchToTab(webDriver, GlobalVariable.navigationDict["Claim"], GlobalVariable.navigationDict["NewNoticeOfLoss"], webDriver.CurrentWindowHandle, webDriver.WindowHandles);
                        currentWindowHandle = webDriver.CurrentWindowHandle;
                        continue;
                    }


                    // Add to the DataTable

                }



                return true;
            }
            catch(Exception ex)
            {
                LogMessage.Log("Error ClaimProcess->ProcessClaim :" + ex.Message);
                LogMessage.Log("Error ClaimProcess->ProcessClaim :" + ex.StackTrace);
                return false;

            }
            finally
            {
                LogMessage.Log("Exiting the excel application.");
                excelWorkBook.Save();
                excelWorkBook.Close(0);
                //Marshal.ReleaseComObject(excelWorkBook);
                excelApp.Quit();
                // Marshal.ReleaseComObject(excelApp);
                webDriver.Close();
                webDriver.Quit();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

       

        

        public static Dictionary<string,string> Navigation(DataTable tblFiltered, IWebDriver webDriver,string policyNumber,string totalBaseAmt,string gadiNumber,CheckBox checkBox,string insType,string causeOfLoss)
        {
            Dictionary<string, string> outDict = new Dictionary<string, string>();
            LogMessage.Log("Clicking on Search icon - PolicyNoSearchIcon");
            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyNoSearchIcon"], GlobalVariable.navigationDict["PolicySearchButton"]);

            //Enter policy number
            LogMessage.Log("Enetr Policy Number - "+ policyNumber);
            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNumber"])).SendKeys(policyNumber);

            //Click on search button
            LogMessage.Log("Clicking on search button - PolicySearchButton");
            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySearchButton"])).Click();

            Thread.Sleep(3000);

            LogMessage.Log("Searcing the result");
            string resultText = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyListTable"])).Text;
            if (resultText.Contains(policyNumber))
            {
                //Select the first policy from search
                //clicking the radio button step
                LogMessage.Log("Seleting first poicy from the search - PolicyNoRadioButton");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNoRadioButton"])).Click();

                if (checkBox.Checked)
                {
                    MessageBox.Show("Please verify the selected policy, then press OK.", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                    


                //Press Coninue button
                LogMessage.Log("Pressing Continue button - PolicyListContinueButton");
                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyListContinueButton"], GlobalVariable.navigationDict["PolicyDateOfLoss"]);

                //Filling date of loss
                LogMessage.Log("Filling Date of loss field - PolicyDateOfLoss");
                string tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyDateOfLoss"]].ToString().Trim();
                //string tempVar = ConvertStringToDate.convertToDate(tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyDateOfLoss"]].ToString().Trim());
                //tempVar = DateTime.Parse(tempVar).ToString("dd/MM/yyyy");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfLoss"])).SendKeys(tempVar);

                //Filling Date of notification
                LogMessage.Log("Filling date of notice field - PolicyDateOfNotification");
                tempVar = ConvertStringToDate.convertToDate(tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyDateOfNotification"]].ToString().Trim());
                //tempVar = DateTime.Parse(tempVar).ToString("dd/MM/yyyy");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfNotification"])).SendKeys(tempVar);

                //Selecting the business type 
                LogMessage.Log("Business type is direct business by default");
                string policyType = policyNumber;


                //Selecting SIF/OIF
                LogMessage.Log("For all claims SIF is selected by default ");
                LogMessage.Log("Policy No."+ tblFiltered.Rows[0]["Policy No."]+Environment.NewLine+"Claim Number "+ tblFiltered.Rows[0]["CLAIM NO."]);
               

                //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDateOfNotification"])).SendKeys(row[GlobalVariable.mappingDict["PolicyDateOfNotification"]].ToString().Trim());

                //Click on retrieve button
                LogMessage.Log("Click on retrieve button -PolicyRetrieve");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyRetrieve"])).Click();

                Thread.Sleep(9000);
                if (webDriver.FindElements(By.XPath(GlobalVariable.navigationDict["ClaimExistsPopup"])).Count != 0)
                {
                    //Enter the vehicle number in Risk Name field
                    LogMessage.Log("Entering vehicle number to the risk name field");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["EnterVehicleNumber"])).SendKeys(gadiNumber);
                    //Search button
                    LogMessage.Log("Click on search button");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SearchVehicle"])).Click();

                    string resultVehicletb = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["VehicleTable"])).Text;
                    if (resultVehicletb.Contains(gadiNumber))
                    {
                        LogMessage.Log("Vehicle info is available clicking on check box");
                        //click checkbox
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["VehicleCheckbox"])).Click();
                        // click continue button
                        LogMessage.Log("Click on continue button");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ContinueButton"])).Click();

                         LogMessage.Log("Updating the Accident description");
                         webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["EnterAccidentDescription"])).SendKeys(causeOfLoss);

                        LogMessage.Log("for loop to count the total outstanding amount");
                        int totalOSAmount = 0;
                       
                        for(int i =0; i< tblFiltered.Rows.Count; i++)
                        {
                            string osAmountString = tblFiltered.Rows[i]["05-Total O/S"].ToString();
                            totalOSAmount = totalOSAmount + (int)Convert.ToInt64(osAmountString);

                        }
                        int totalBaseAmtnum = (int)Convert.ToInt64(totalBaseAmt);
                        if(totalBaseAmtnum == totalOSAmount)
                        {
                            for (int rwCount = 0; rwCount < tblFiltered.Rows.Count; rwCount++)
                            {
                                LogMessage.Log("Entering Damage information for the policy" + policyNumber);
                                //string loss_description = tblFiltered.Rows[rwCount]["Loss_Description"].ToString();
                                string outstandingAmount = tblFiltered.Rows[rwCount]["05-Total O/S"].ToString();
                                int outstandingAmountInt = (int)Convert.ToInt64(outstandingAmount);
                                if (outstandingAmountInt != null || outstandingAmountInt > 0)
                                {
                                    LogMessage.Log("Cliking on plus icon to add damage info");
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddDamageInfo"])).Click();

                                    LogMessage.Log("Selecting first party or 3rd party based on claimant type");
                                    switch (tblFiltered.Rows[rwCount]["Claimant Type"]) 
                                    {
                                        case "TPI":
                                            LogMessage.Log("Claimant type is 3rd party bodily injury as per given input sheet");
                                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageThirdPartyParty"])).Click();
                                            break;

                                        case "TPD":
                                            LogMessage.Log("Claimant type is 3rd party property damage as per given input sheet");
                                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageThirdPartyParty"])).Click();
                                            break;

                                        case "OD":
                                            LogMessage.Log("Claimant type is first party own damage as per given input sheet");
                                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageFirstParty"])).Click();
                                            break;

                                        default:
                                            LogMessage.Log("Unable to detect 1st party or 3rd party as input sheet is not provided properly");
                                            outDict["Result"] = "FAIL";
                                            outDict["Comment"] = "Unable to find out 1st party or second party based on input sheet";
                                            break;
                                    }

                                    LogMessage.Log("Selecting Liabality as Damage type as vehicle by default");
                                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["PolicyDamageType"], "Vehicle");

                                    
                                    string vehicle_number = tblFiltered.Rows[rwCount]["Veh_No."].ToString();
                                    string claimant_type = tblFiltered.Rows[rwCount]["Claimant Type"].ToString();
                                    string damage_item = vehicle_number + "-" + claimant_type;
                                    LogMessage.Log("Entering the Damage Item as "+ damage_item);
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageItem"])).SendKeys(damage_item);
                                    LogMessage.Log("Selecting new claimant");
                                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], "*New Claimant");
                               
                                    LogMessage.Log("Entering claimant to the text field");
                                    string ct_name = tblFiltered.Rows[rwCount]["Claimant"].ToString();
                                    string claimant = ct_name.Substring(4);
                                    LogMessage.Log("Entering the claimant name as given in the input sheet" + claimant);
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNewClaimantSearchName"])).SendKeys(claimant);

                                    LogMessage.Log("Clicking search button");
                                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageNewClaimantSearch"])).Click();

                                    string resultClaimanttb = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimantTable"])).Text;
                                    if(resultClaimanttb == "")
                                    {
                                        LogMessage.Log("Could not find the claimant in ebao system");
                                        LogMessage.Log("Creating a new claimant");
                                        LogMessage.Log("Not exist in common party");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["NotExistInCommonParty"])).Click();
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PartyName"])).SendKeys(claimant);
                                        LogMessage.Log("adding contact info");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfo"])).Click();
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoContactNumber"])).SendKeys("-");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoContactEmail"])).SendKeys("-");

                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoSave"])).Click();

                                        LogMessage.Log("Adding address info section");
                                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["AddressTypeDropdown"], "Other");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddressPostalCode"])).SendKeys(" -");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddressAdress1"])).SendKeys("-");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddressSaveButton"])).Click();
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SelectASClaimant"])).Click();

                                        LogMessage.Log("Done with creating a new claimant proceeding further");
                                    }

                                    string resultClaimantName = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimantTableClaimantName"])).Text;
                                    if(resultClaimanttb.Contains(resultClaimantName))
                                    {
                                        LogMessage.Log("Select the first claimant ");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimantSelectRadiobtn"])).Click();

                                        LogMessage.Log("Clicking on add to claim party");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyAddToClaimParty"])).Click();

                                        LogMessage.Log("Clicking on select as claimant");
                                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicySelectAsClaimant"])).Click();
                                    }
                                   
                                    
                                      
                                    



                                }

                            }
                        }
                        else
                        {
                            outDict["Result"] = "FAIL";
                            outDict["Comment"] = "Total outstanding amount in settlement and main sheet is not matching hence didn't proceed further";
                        }

                       

                    }
                    else
                    {
                         outDict["Result"] = "FAIL";
                         outDict["Comment"] = "Vehicle info was not found in eboa portal";
                    }

                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimExistsPopup"])).Click();

             }
              else
                {
                    outDict["Result"] = "FAIL";
                    outDict["Comment"] = "Policy number doesn't exist in ebao portal ";
                }

                
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(90));
                wait.Until(ExpectedConditions.ElementExists(By.XPath(GlobalVariable.navigationDict["PolicyContinue"])));

                //Add to select vessel value

                //MessageBox.Show("Please select the Risk information for : " + tblVesselFiltered.Rows[0][GlobalVariable.mappingDict["VesselName"]].ToString().Trim() + ", then press OK.", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);


                LogMessage.Log("Click on continue button");
                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyContinue"], GlobalVariable.navigationDict["PolicyAccidentDesc"]);
                Thread.Sleep(3000);



                //Fill Accident description
                LogMessage.Log("Filling accident description. - PolicyAccidentDesc");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyAccidentDesc"])).SendKeys(tblFiltered.Rows[0][GlobalVariable.mappingDict["PolicyAccidentDesc"]].ToString().Trim());

                //Adding damage info
                LogMessage.Log("Adding damage info -PolicyAddDamageInfo");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyAddDamageInfo"])).Click();

                //selecting the party type
                LogMessage.Log("Selecting the party type(defaulting to first party) -PolicyDamageFirstParty");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageFirstParty"])).Click();

                //Drop down select
                LogMessage.Log("Selecting Liabality as Damage type");
                Dropdown.Select(webDriver, GlobalVariable.navigationDict["PolicyDamageType"], "Marine");

                LogMessage.Log("Selecting Claimant");
                IWebElement l = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PoicyDamageClaimant"]));
                //object of SelectElement
                SelectElement s = new SelectElement(l);
                //Options method to get all options
                IList<IWebElement> els = s.Options;
                //count options
                bool claimantFlag = false;
                int e = els.Count;
                for (int j = 0; j < e; j++)
                {
                    if ((els.ElementAt(j).Text).ToString().Contains(tblFiltered.Rows[0][GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim()))
                    {
                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], els.ElementAt(j).Text);
                        claimantFlag = true;
                        break;
                    }

                }
                if (!claimantFlag)
                {
                    LogMessage.Log("Claimant Not found creating new claimant.");
                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], "*New Claimant");
                    LogMessage.Log("Waiting for the appearence of search button");
                    WebDriverWait waitTime = new WebDriverWait(webDriver, TimeSpan.FromSeconds(60));
                    waitTime.Until(ExpectedConditions.ElementExists(By.XPath(GlobalVariable.navigationDict["PolicyDamageNewClaimantSearch"])));
                    //Thread.Sleep(8000);
                    LogMessage.Log("Entering the claimant name");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNewClaimantTypeOrg"])).Click();
                    string claimantName = tblFiltered.Rows[0][GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim();
                    if (claimantName.Contains("1"))
                    {
                        claimantName = claimantName.Split("-")[1].ToString().Trim();
                    }
                    LogMessage.Log("Clicking PolicyNewClaimantSearchName");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNewClaimantSearchName"])).SendKeys(claimantName);
                    LogMessage.Log("Clicking PolicyDamageNewClaimantSearch");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageNewClaimantSearch"])).Click();
                    DialogResult dr = MessageBox.Show("Please select the claimant.  "+Environment.NewLine+"Press YES, If you selected claimant" + Environment.NewLine + "Press No, To create claimant", "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dr == DialogResult.Yes)
                    {
                        LogMessage.Log("Clicking AddToClaimParty");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicyAddToClaimParty"], GlobalVariable.navigationDict["PolicySelectAsClaimant"]);
                        LogMessage.Log("Clicking PolicySelectAsClaimant");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["PolicySelectAsClaimant"], GlobalVariable.navigationDict["PolicyDamageSave"]);

                    }
                    else
                    {
                        LogMessage.Log("Adding new claimant " + claimantName);
                        LogMessage.Log("Clicking  NotExistInCommonParty");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["NotExistInCommonParty"], GlobalVariable.navigationDict["PartyTypeOrganization"]);
                        Thread.Sleep(2000);

                        LogMessage.Log("Selecting organization");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PartyTypeOrganization"])).Click();

                        LogMessage.Log("Filling PartyName ");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PartyName"])).SendKeys(claimantName);

                        LogMessage.Log("Clicking  Contact info");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["AddContactInfo"], GlobalVariable.navigationDict["AddContactInfoContactNumber"]);

                        LogMessage.Log("Filling AddContactInfoContactNumber ");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoContactNumber"])).SendKeys("NA");

                        LogMessage.Log("Filling AddContactInfoContactEmail ");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoContactEmail"])).SendKeys("NA");

                        LogMessage.Log("Clicking AddContactInfoSave");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddContactInfoSave"])).Click();

                        LogMessage.Log("Clicking AddAddressInfo");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddAddressInfo"])).Click();

                        //Address type drop down Work
                        LogMessage.Log("Selecting AddressType dropdown - Work");
                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["AddAddressInfoAddressType"], "Other");

                        LogMessage.Log("Filling  AddAddressInfoPostalCode");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddAddressInfoPostalCode"])).SendKeys("NA");

                        LogMessage.Log("Filling  AddAddressInfoAddressOne");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddAddressInfoAddressOne"])).SendKeys("NA");

                        LogMessage.Log("Clicking AddAddressInfoSave");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["AddAddressInfoSave"])).Click();

                        MessageBox.Show("Please verify the information on the screen, then press OK", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        //AddAsClaimantNew
                        LogMessage.Log("Clicking AddAsClaimantNew");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["AddAsClaimantNew"], GlobalVariable.navigationDict["PolicyDamageSave"]);
                        Thread.Sleep(2000);
                    }

                }


                //Selecting claimant

                //Dropdown.Select(webDriver, GlobalVariable.navigationDict["PoicyDamageClaimant"], row[GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim());

                //Clicking on save

                //MessageBox.Show("Please verify the policy and damage information then press OK");
                LogMessage.Log("Clicking on save - PolicyDamageSave");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyDamageSave"])).Click();

                if (checkBox.Checked)
                {
                    MessageBox.Show("Please verify the information, then press OK.", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }


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
                ClaimNumber = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["GetClaimNumber"])).Text;
                if(!string.IsNullOrEmpty(ClaimNumber))
                {
                    ClaimNumber= ClaimNumber.Split(" ")[1];
                }
                LogMessage.Log("Claim number is" + ClaimNumber);

                string policyNo = tblFiltered.Rows[0][GlobalVariable.mappingDict["DefaultClaimNumber"]].ToString().Trim();
                policyNo = "PSTMIG - " + policyNo.ToString().Trim();

                //Filling Refnumber
                LogMessage.Log("Filling reference Number: " + policyNo);
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ReferenceNo"])).SendKeys(policyNo);
                bool causeOfLossFlag = false;
                //Selecting Cause of loss 
                LogMessage.Log("Selcting cause of loss CauseofLossDropdown");
                GlobalVariable.errorStatus = false;

                causeOfLoss = causeOfLoss.Split('/')[0];
                GlobalVariable.errorStatus = false;
                l = webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CauseofLossDropdown"]));
                //object of SelectElement
                s = new SelectElement(l);
                //Options method to get all options
                els = s.Options;
                //count options
                e = els.Count;
                for (int j = 0; j < e; j++)
                {
                    if ((els.ElementAt(j).Text).ToString().ToLower().Contains(causeOfLoss.Trim().ToLower()) || (causeOfLoss.Trim().ToLower().Contains((els.ElementAt(j).Text).ToString().ToLower())))
                    {
                        LogMessage.Log("Selecting "+ (els.ElementAt(j).Text).ToString());
                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["CauseofLossDropdown"], els.ElementAt(j).Text.ToString());
                        causeOfLossFlag = true;
                        break;
                    }

                   

                }


                //Dropdown.Select(webDriver, GlobalVariable.navigationDict["CauseofLossDropdown"], causeOfLoss.ToString().Trim());
                if (GlobalVariable.errorStatus || causeOfLossFlag ==false)
                {
                    LogMessage.Log(causeOfLoss+" not found, selecting the default one :"+ GlobalVariable.defaultValues["CauseofLossDropdown"].ToString().Trim());
                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["CauseofLossDropdown"], GlobalVariable.defaultValues["CauseofLossDropdown"].ToString().Trim());
                }
                //Selecting subclaim insured object
                LogMessage.Log("Selecting subclaim insured object");
                GlobalVariable.errorStatus = false;
                Dropdown.Select(webDriver, GlobalVariable.navigationDict["SubClaimInsuredObject"], tblFiltered.Rows[0][GlobalVariable.mappingDict["PoicyDamageClaimant"]].ToString().Trim());
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
                    LogMessage.Log("Unable to find the Subclaim owner, " + GlobalVariable.configDict["Username"].ToString().Trim()+"Using the default one"+ GlobalVariable.defaultValues["DefaultUsernameForSuClaimOwner"].ToString());
                    Dropdown.Select(webDriver, GlobalVariable.navigationDict["subclaimClaimOwner"], GlobalVariable.defaultValues["DefaultUsernameForSuClaimOwner"].ToString().Trim());
                }

                //Select subClaimCoverageSelect
                LogMessage.Log("Selecting the subClaimCoverageSelect");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["subClaimCoverageSelect"])).Click();


                //  MessageBox.Show("Pease verify the info then press OK");

                if (checkBox.Checked)
                {
                    MessageBox.Show("Please verify the information, then press OK.", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                //Save buttom ClaimSave
                LogMessage.Log("Clicking ClaimSave");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimSave"])).Click();

                //submit button
                LogMessage.Log("Clicking ClaimSubmit");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["ClaimSubmit"])).Click();

                // SubClaimManualAssignment
                LogMessage.Log("Clicking SubClaimManualAssignment");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimManualAssignment"])).Click();

                //SubClaimTaskTransfer
                LogMessage.Log("Clicking SubClaimTaskTransfer");
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubClaimTaskTransfer"])).Click();


                //BackToClaimTaskTransfer
                LogMessage.Log("Clicking BackToClaimTaskTransfer");
                Click.ButtonClick(webDriver, GlobalVariable.navigationDict["BackToClaimTaskTransfer"], GlobalVariable.navigationDict["ClaimOptionLOS"]);
                Thread.Sleep(2000);
                //int lossExpenses = 0;
                LogMessage.Log("Verifying the Amount from base sheet with settlement sheet");

                LogMessage.Log("Updating the out standing amount amount");
                double totalAmt = 0;
                string totalLoss = null;
                string subrogationLoss = null;
                bool lossFlag = false;
                bool subrogationFlag = false;
                foreach (DataRow row in tblFiltered.Rows)
                {
                    totalAmt += Convert.ToDouble(row[GlobalVariable.mappingDict["TotalAmt"].ToString().Trim()]);
                    if ((row[GlobalVariable.mappingDict["SettleType"]]).ToString().Trim().ToLower() == GlobalVariable.defaultValues["SettleTypeLoss"].ToString().Trim().ToLower())
                    {
                        totalLoss = row[GlobalVariable.mappingDict["TotalAmt"].ToString().Trim()].ToString().Trim();
                        lossFlag = true;
                    }
                    if ((row[GlobalVariable.mappingDict["SettleType"]]).ToString().Trim().ToLower() == GlobalVariable.defaultValues["SettleTypeSubrogation"].ToString().Trim().ToLower())
                    {
                        subrogationLoss = row[GlobalVariable.mappingDict["TotalAmt"].ToString().Trim()].ToString().Trim();
                        subrogationFlag = true;
                    }
                }
                if (totalBaseAmt == Convert.ToString(totalAmt))
                {
                    LogMessage.Log("Updating the outstanding amounts");

                    //If loss is found then update the loss amount
                    if (lossFlag)
                    {
                        LogMessage.Log("Clicking on ReserveLossUpdate");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["ReserveLossUpdate"], GlobalVariable.navigationDict["OutStandingAmt"]);

                        LogMessage.Log("Filling outstanding amount OutStandingAmt");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["OutStandingAmt"])).SendKeys(totalLoss);

                        LogMessage.Log("Clicking OutStandingSubmit");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["OutStandingSubmit"], GlobalVariable.navigationDict["OpenReserveLink"]);


                        LogMessage.Log("Clicking OutStandingPopUpBox");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["OutStandingPopUpBox"], GlobalVariable.navigationDict["OpenReserveLink"]);
                        lossFlag = false;
                    
                    }
                    //If subrogation found then update the amount - Settlement Tye: Recover from T.P.
                    if (subrogationFlag)
                    {

                        if (subrogationLoss.Contains("-"))
                        {
                            LogMessage.Log("Clicking OpenReserveLink");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["OpenReserveLink"], GlobalVariable.navigationDict["SubrogationRadiobox"]);
                            Thread.Sleep(2000);
                            LogMessage.Log("Clicking SubrogationRadiobox");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubrogationRadiobox"])).Click();



                            LogMessage.Log("Clicking Submit");
                            //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["LossExpenseSubmit"])).Click();
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["LossExpenseSubmit"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                            LogMessage.Log("Click SubrogateUpdate");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["SubrogateUpdate"], GlobalVariable.navigationDict["SubrogationAmtUpdate"]);

                            LogMessage.Log("Entering SubrogationAmtUpdate");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubrogationAmtUpdate"])).SendKeys(subrogationLoss);

                            LogMessage.Log("Clicking the SubrogateAmtSubmitbtn");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["SubrogateAmtSubmitbtn"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                        }
                        else
                        {
                            LogMessage.Log("Clicking OpenReserveLink");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["OpenReserveLink"], GlobalVariable.navigationDict["SubrogateLossRadioBox"]);
                            Thread.Sleep(2000);
                            LogMessage.Log("Clicking SubrogateLossRadioBox");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubrogateLossRadioBox"])).Click();

                            LogMessage.Log("Clicking SubrogateLossSubmit");
                            //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["LossExpenseSubmit"])).Click();
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["SubrogateLossSubmit"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                            LogMessage.Log("Click SubrogateLossUpdate");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["SubrogateLossUpdate"], GlobalVariable.navigationDict["SubrogateLossAmtUpdate"]);

                            LogMessage.Log("Entering SubrogateLossAmtUpdate");
                            webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["SubrogateLossAmtUpdate"])).SendKeys(subrogationLoss);

                            LogMessage.Log("Clicking the SubrogateLossAmtSubmit");
                            Click.ButtonClick(webDriver, GlobalVariable.navigationDict["SubrogateLossAmtSubmit"], GlobalVariable.navigationDict["ClaimOptionLOS"]);


                        }

                    }
                    //lossExpenses+=Con
                    if ((Convert.ToString(totalAmt - (Convert.ToDouble(totalLoss)+Convert.ToDouble(subrogationLoss))))!= "0"){
                        LogMessage.Log("Clicking OpenReserveLink");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["OpenReserveLink"], GlobalVariable.navigationDict["LossExpenseRadioBox"]);
                        Thread.Sleep(2000);
                        LogMessage.Log("Clicking LossExpenseRadioBox");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["LossExpenseRadioBox"])).Click();



                        LogMessage.Log("Clicking LossExpenseSubmit");
                        //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["LossExpenseSubmit"])).Click();
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["LossExpenseSubmit"], GlobalVariable.navigationDict["ClaimOptionLOS"]);

                        LogMessage.Log("Click LossExpenseUpdateAmt");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["LossExpenseUpdateAmt"], GlobalVariable.navigationDict["LossExpenseOSReserve"]);
                        totalLoss = Convert.ToString(totalAmt - (Convert.ToDouble(totalLoss) + Convert.ToDouble(subrogationLoss)));
                        LogMessage.Log("Entering loss  expense amount LossExpenseAmt");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["LossExpenseOSReserve"])).SendKeys(totalLoss);

                        LogMessage.Log("Clicking the LossExpenseAmtSubmit");
                        Click.ButtonClick(webDriver, GlobalVariable.navigationDict["LossExpenseAmtSubmit"], GlobalVariable.navigationDict["ClaimOptionLOS"]);
                    }
                        

                    outDict["Result"] = "PASS";
                    outDict["Comment"] = "OK";



                }
                else
                {
                    LogMessage.Log("Amount mismatch for Policy Number "+policyNumber);
                    outDict["Result"] = "FAIL";
                    outDict["Comment"] = "Out standing amount is different in claim and settlement sheet.";
                }
                //Entering the 
                return outDict;

                // return true;
            }
            else
            {
                LogMessage.Log("Policy not found in eBAO system");
                outDict["Result"] = "FAIL";
                outDict["Comment"] = "Policy not found in eBAO system";
                webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["PolicyNumber"])).Clear();
                return outDict;
                //GlobalVariable.AddDataToSummaryTabe(CurrentPolicyNo, claimNo, "Policy Number not avialable ", "PASS", "Claim");
               // continue;
            }

        }
    }
}
