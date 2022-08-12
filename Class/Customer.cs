using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace III_ProjectOne.Class
{
    internal class Customer
    {
        public static int sleepCtr = 1000;
        public static string tempWaitElement = null;
        public static string tempXpath = null;
        public static string type = "Customer";
        public static bool CustomerSearch(IWebDriver webDriver)
        {
            
            try
            {
                string windowsHandle = webDriver.CurrentWindowHandle;
                //Click on policy administration
                LogMessage.Log("Clicking on customer from policy administration screen");
                tempXpath = GlobalVariable.navigationDict["Customer"];
                tempWaitElement = GlobalVariable.navigationDict["CustomerSearch"];
                
                var resultFlag = Click.ButtonClick(webDriver, tempXpath, tempWaitElement);
                Thread.Sleep(sleepCtr);
                //webDriver.FindElement(By.CssSelector("[accesskey='l']")).Click();

                if (resultFlag)
                {
                    LogMessage.Log("Clicking on Customer successful...");
                    LogMessage.Log("Searching for customer");


                    foreach (DataRow row in GlobalVariable.dtClaimData.Rows)
                    {
                        GlobalVariable.cancellationToken.ThrowIfCancellationRequested();
                        LogMessage.Log("---------------------------------------------");
                        GlobalVariable.errorStatus = false;
                       

                        bool result = SearchCustomer(webDriver,row);
                        //CreateCustomer(webDriver, row);

                         if(!result && !GlobalVariable.errorStatus)
                         {
                             result = CreateCustomer(webDriver, row);
                         }
 



                    }

                    
                    Thread.Sleep(sleepCtr);
                    return true;
                   
                }

                else
                {
                    LogMessage.Log("Condition failed..");
                    return false;
                }

                //return true;
            }
            catch(Exception ex)
            {
                LogMessage.Log("Error: Customer->CustomerSearch -" + ex.Message);
                LogMessage.Log("Error: Customer->CustomerSearch -" + ex.StackTrace);
                return false;

            }
        }

        public static bool SearchCustomer(IWebDriver webDriver,DataRow  row)
        {
            string name = row[GlobalVariable.mappingDict["CustomerName"]].ToString().Trim();
            try {

                LogMessage.Log("Searching customer " + name);

                //Select dropdown
                LogMessage.Log("Clicking on  CustomerTypeDropdown");
                tempXpath = GlobalVariable.navigationDict["CustomerTypeDropdown"];
                
                Dropdown.Select(webDriver, tempXpath, row[GlobalVariable.mappingDict["CustomerType"]].ToString().Trim());

                //enter customer name
                //LogMessage.Log("Entering CustomerName");
                tempXpath = GlobalVariable.navigationDict["CustomerName"];
                LogMessage.Log("Clearing Customer Name field.");
                webDriver.FindElement(By.XPath(tempXpath)).Clear();
                LogMessage.Log("Entering CustomerName for search");
                webDriver.FindElement(By.XPath(tempXpath)).SendKeys(name);

                //Click on search
                LogMessage.Log("Clicking on Search button");
                tempXpath = GlobalVariable.navigationDict["CustomerSearchButton"];
                webDriver.FindElement(By.XPath(tempXpath)).Click();

                Thread.Sleep(5000);

                //Check Table value
                tempXpath = GlobalVariable.navigationDict["CustomerSearchResult"];
                LogMessage.Log("Fetching CustomerSearchResult value to check is customer exist");
                string resultText= webDriver.FindElement(By.XPath(tempXpath)).Text;
                if (resultText.Contains(name))
                {
                    LogMessage.Log("Customer data found, skipping creation.");
                    GlobalVariable.AddDataToSummaryTabe(name,type ,"Customer data found, skipping creation", "PASS",type);
                    return true;
                }
                else
                {
                    return false;
                }
                
            
            }
            catch(Exception ex)
            {

                GlobalVariable.AddDataToSummaryTabe(name,type, "Error while searching for the customer", "UNKNOWN",type);
                LogMessage.Log("Error in searching for customer -" + name);
                LogMessage.Log("Error: searchCustomer - "+ex.Message);
                LogMessage.Log("Error: searchCustomer - "+ex.StackTrace);
                GlobalVariable.errorStatus = true;
                return false;
            }
        }

        public static bool CreateCustomer(IWebDriver webDriver, DataRow row)
        {
            string name = row[GlobalVariable.mappingDict["CustomerName"]].ToString().Trim();
            string tempVar=null;
            LogMessage.Log("Creating customer profile for " + name);
            try
            {
                DataTable tblFiltered = new DataTable();
                try
                {
                    LogMessage.Log("Filtering PST Data file for customer name");
                    //filter second dt for the register number if found tblfilter will contain 1 row
                    tblFiltered = GlobalVariable.dtCustomerAgentData.AsEnumerable()
                                .Where(r => r.Field<string>(GlobalVariable.mappingDict["CustomerNamePST"].ToString().Trim()) == name)
                                .CopyToDataTable();
                }
                catch (Exception ex)
                {
                    LogMessage.Log("Error: Encountered issue while  searching for customer: " +name+ " in pst file.");
                    LogMessage.Log("Error: " + ex.Message);
                    LogMessage.Log("Error: " + ex.StackTrace);
                    GlobalVariable.AddDataToSummaryTabe(name,type, "Unable to find data", "FAIL",type);
                    return false;
                }


                if(tblFiltered.Rows.Count > 0)
                {
                    //customer type
                    //Select dropdown
                    LogMessage.Log("Cicking on CustomerAddButton");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CustomerAddButton"])).Click();
                  
                    
                    if(row[GlobalVariable.mappingDict["CustomerType"]].ToString().Trim().ToLower()=="organization")
                    {
                        //Select Organization from dropdown
                        LogMessage.Log("Selecting Customer type dropdown");
                        tempXpath = GlobalVariable.navigationDict["CreateCustomerTypeOrganization"];
                        Dropdown.Select(webDriver, tempXpath, row[GlobalVariable.mappingDict["CustomerType"]].ToString().Trim());
                        //fetch column name from the mapping dict
                        
                        tempVar = GlobalVariable.mappingDict["CustomerNamePST"].ToString().Trim();
                        tempVar = (tblFiltered.Rows[0][tempVar]).ToString().Trim();

                        //Write into Company name field
                        LogMessage.Log("Filling CreateCustomerCompanyName");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerCompanyName"])).SendKeys(tempVar);

                        //fetch column name from the mapping dict
                        tempVar = GlobalVariable.mappingDict["CustomerIDNumber"].ToString().Trim();
                        tempVar = (tblFiltered.Rows[0][tempVar]).ToString().Trim();
                        //Regstration number
                        LogMessage.Log("Filling CreateCustomerRegistrationNumber");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerRegistrationNumber"])).SendKeys(tempVar);

                        //MessageBox.Show("Please validate");

                        //click on save
                        LogMessage.Log("Clicking on CreateCustomerSave");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerSave"])).Click();

                        //Fill RegDate
                        tempVar = row[GlobalVariable.mappingDict["CustomerDateofReg"]].ToString().Trim();
                        LogMessage.Log("Converting to date "+tempVar);
                        var curDate = ConvertStringToDate.convertToDate(tempVar);
                        curDate = DateTime.Parse(curDate).ToString("dd/MM/yyyy");
                        //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerDateOfReg"])).Click();
                        LogMessage.Log("Filling data for CreateCustomerDateOfReg");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerDateOfReg"])).SendKeys(curDate);


                       

                    }

                    //Address and Contact info shares the same xpath and values for both organization and individual type.
                    //Click on Add address button
                    LogMessage.Log("Clicking on CreateCustomerAddAddress");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddAddress"])).Click();
                    Thread.Sleep(2000);
                    //Get country from the PST file
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressTypeCustomer"]].ToString().Trim();
                    if (tempVar.ToUpper() == "SG")
                    {
                        //Click on primary
                        LogMessage.Log("Clicking on CreateCustomerAddressPrimary");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddressPrimary"])).Click();
                    }
                    else
                    {
                        //Click on Foreign address
                        LogMessage.Log("Clickin on CreateCustomerAddressForeign");
                        webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddressForeign"])).Click();

                    }

                    //Enter PIN Code
                   
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerPostalCode"]].ToString().Trim();

                    if (string.IsNullOrEmpty(tempVar))
                    {

                        //tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressTypeCustomer"]].ToString().Trim();
                        tempVar = GlobalVariable.defaultValues["CustomerPostalCode"];
                    }
                    LogMessage.Log("Filling data for CreateCustomerPostalCode");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerPostalCode"])).SendKeys(tempVar);

                    //Enter Addr1
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressOne"]].ToString().Trim();

                    if (string.IsNullOrEmpty(tempVar))
                    {
                        tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressTypeCustomer"]].ToString().Trim();
                    }
                    LogMessage.Log("Filling data for CreateCustomerAddrOne");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddrOne"])).SendKeys(tempVar);

                    //Address2
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressTwo"]].ToString().Trim();
                    LogMessage.Log("Filling data for CreateCustomerAddrTwo");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddrTwo"])).SendKeys(tempVar);

                    //Address3
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressThree"]].ToString().Trim();
                    LogMessage.Log("Filling data for CreateCustomerAddrThree");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddrThree"])).SendKeys(tempVar);

                    if (tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressTypeCustomer"]].ToString().Trim() == "SG")
                    {
                        //Address4
                        tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerAddressFour"]].ToString().Trim();
                        tempVar = GlobalVariable.countryDict[tempVar];
                        LogMessage.Log("Selecting dropdown CreateCustomerAddrFour");
                        Dropdown.Select(webDriver, GlobalVariable.navigationDict["CreateCustomerAddrFour"], tempVar);

                    }

                    //Click on Save
                    LogMessage.Log("Clicking on CreateCustomerSaveTwo");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerSaveTwo"])).Click();
                    Thread.Sleep(2000);

                    //Fill Contact info
                    //As of 06-07-2022 we do not have mapping for primarycontact check box  and contact person field. skipping those 2 here

                    //Click on Add Contact Button
                    LogMessage.Log("Clicking on CreateCustomerAddContact");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerAddContact"])).Click();

                    //Mobile
                    LogMessage.Log("Filling data for CreateCustomerMobile");
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerMobile"]].ToString().Trim();
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerMobile"])).SendKeys(tempVar);


                    //WorkHome tel
                    LogMessage.Log("Filling data for CreateCustomerWorkHomeTel");
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerWorkHomeTel"]].ToString().Trim();
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerWorkHomeTel"])).SendKeys(tempVar);

                    //Fax
                    LogMessage.Log("Filling data for CreateCustomerFax");
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerFax"]].ToString().Trim();
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerFax"])).SendKeys(tempVar);

                    //Email
                    LogMessage.Log("Filling data for CreateCustomerEmail");
                    tempVar = tblFiltered.Rows[0][GlobalVariable.mappingDict["CustomerEmail"]].ToString().Trim();
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerEmail"])).SendKeys(tempVar);


                    //Click on Save button
                    LogMessage.Log("Clicking CreateCustomerSaveThree");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerSaveThree"])).Click();
                    Thread.Sleep(2000);

                    //Click on submit
                    LogMessage.Log("Clicking CreateCustomerSubmit");
                    webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerSubmit"])).Click();

                    //Click on Exit
                    LogMessage.Log("Clicking CreateCustomerSubmit");
                    //webDriver.FindElement(By.XPath(GlobalVariable.navigationDict["CreateCustomerExit"])).Click();
                    bool result = Click.ButtonClick(webDriver, GlobalVariable.navigationDict["CreateCustomerExit"], GlobalVariable.navigationDict["CustomerName"]);

                    if (!result)
                    {

                        webDriver.Navigate().Back();
                        webDriver.Navigate().Refresh();


                    }

                }

                return true;
            }
            catch(Exception ex)
            {
                LogMessage.Log("Error: Customer Creation - " + ex.Message);
                LogMessage.Log("Error: Customer Creation - " + ex.StackTrace);
                GlobalVariable.AddDataToSummaryTabe(name,type,"Error occured while creating the customer", "FAIL",type);
                return false;
            }

        }
    }
}
