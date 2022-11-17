using III_ProjectOne.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace III_ProjectOne
{
    public partial class Form1 : Form
    {
        private Thread thread;
        
        CancellationTokenSource cancellationTokenSource;
        //CancellationToken cancellationToken;

        public Form1()
        {
            InitializeComponent();
            

            try
            {
                if (!Directory.Exists(@"C:\III_ProjectOne\Log"))
                {
                    Directory.CreateDirectory(@"C:\III_ProjectOne\Log");
                }
                //string logFileName = @"C:\Log\Log" + DateTime.Now.ToShortDateString().ToString() + ".txt";
                string[] start = {"####################################", DateTime.Now + ": Application initialized...." };
                File.AppendAllLines(GlobalVariable.logFileName, start);
                radioButtonCustomer.Enabled = false;
                radioButtonAgent.Enabled = false;
                endorsementButton.Enabled = false;
                radioButtonClaim.Checked = true;
                textBoxFilePath.Enabled = false;
                buttonBrowse.Enabled = false;


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {

                buttonStart.Enabled = false;    
                if (cancellationTokenSource == null)
                {
                    
                    cancellationTokenSource = new CancellationTokenSource();
                    GlobalVariable.cancellationToken = cancellationTokenSource.Token;
                     Task.Run((Action)initialize, GlobalVariable.cancellationToken);
                }
                //Thread.CurrentThread.IsBackground = true;
                //Reading Config excel.
                

        }

        private void labelProgress_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonCustomer_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonAgent_CheckedChanged(object sender, EventArgs e)
        {

        }

        

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }


            if (GlobalVariable.m_driver == null)
            {
            }
            else
            {
                GlobalVariable.m_driver.Quit();
            }
            
            LabelText.UpdateText(Textlabel, "Execution stopped, press start button to start the execution.");
            var chromeDriverProcesses = Process.GetProcesses().
              Where(pr => pr.ProcessName == "chromedriver"); // without '.exe'

            foreach (var process in chromeDriverProcesses)
            {
                process.Kill();
            }
            buttonStart.Enabled = true;


            //Application.Exit();
        }

        private void initialize()
            {
                try {
                    var curExecDir = Environment.CurrentDirectory;
                    DataTable dataTable = new DataTable();

                
                   
                    LabelText.UpdateText(Textlabel, "Reading config file");
                    LogMessage.Log("Reading config file...");

                    string path = "C:\\III_ProjectOne\\Config_motor.xlsx";
                    dataTable = ProcessExceltoDt.ConvertToDT(path, "Sheet1", Textlabel);
                    //Converting config excel file--> Datatable to DIctionary
                    GlobalVariable.errorStatus = false;
                    GlobalVariable.configDict = ConvertDtToDict.ConvertToDictionary(dataTable);
                    dataTable = null;


                    if (GlobalVariable.errorStatus == true)
                    {
                        MessageBox.Show("Error while reading config file, aborting execution.");
                        LogMessage.Log("Error occured while processing config file, aborting execution..");
                        Application.Exit();
                    }
                    else
                    {

                    RadioButton radioBtn = this.Controls.OfType<RadioButton>()
                                                   .Where(x => x.Checked).FirstOrDefault();

                    switch (radioBtn.Text)
                    {
                        case "Agent":
                            break;

                        case "Check Endorsement":
                            LogMessage.Log("Endorsement selected");
                            Endorsement en = new Endorsement();
                            en.start(textBoxFilePath.Text,Textlabel);
                            break;

                        case "Customer":
                            LogMessage.Log("Selected Option Customer.");
                            //toolTipBrowse.SetToolTip = (this.buttonBrowse, "This tool tip is for test" + Environment.NewLine + "purpose");
                            ReadNavigationAndMapping("Customer", Textlabel);
                            if (GlobalVariable.errorStatus)
                            {
                                MessageBox.Show("Error while reading Mapping or Navigation file, aborting execution." + Environment.NewLine + "Check log for more info.");
                                Application.Exit();
                            }
                            ReadInputFile("Customer");
                            if (GlobalVariable.dtClaimData.Rows.Count > 0 && GlobalVariable.dtCustomerAgentData.Rows.Count > 0)
                            {
                                //Adding columns to the summary dt
                                GlobalVariable.dtSummaryTable.Columns.Add("Name");
                                GlobalVariable.dtSummaryTable.Columns.Add("Type");
                                GlobalVariable.dtSummaryTable.Columns.Add("Remarks");
                                GlobalVariable.dtSummaryTable.Columns.Add("Status");


                                ProcessData process = new ProcessData();
                                
                              

                                process.startProcessing(Textlabel, "Customer",EMVcheckBox);
                            }
                            else
                            {
                                LogMessage.Log("Rows count is 0 in either dtclaimsData or in agentdata pst file...");
                            }



                            break;

                        case "Claim":

                            ReadNavigationAndMapping("Claim", Textlabel);
                            if (GlobalVariable.errorStatus)
                            {
                                MessageBox.Show("Error while reading Mapping or Navigation file, aborting execution." + Environment.NewLine + "Check log for more info.");
                                Application.Exit();
                            }
                            ReadInputFile("Claim");
                            if (GlobalVariable.dtCustomerAgentData.Rows.Count > 0)
                            {
                                //Adding columns to the summary dt
                               /* GlobalVariable.dtSummaryTable.Columns.Add("Policy Number");
                                GlobalVariable.dtSummaryTable.Columns.Add("Claim Number");                               
                                GlobalVariable.dtSummaryTable.Columns.Add("Status");
                                GlobalVariable.dtSummaryTable.Columns.Add("Remarks");*/

                                ProcessData process = new ProcessData();

                                process.startProcessing(Textlabel, "Claim",EMVcheckBox);
                            }
                            else
                            {
                                LogMessage.Log("Rows count is 0 in check tool input settlement sheetfile ...");
                            }

                            break;



                    }

                    
                            //Writing SUmmary File
                            
                        if (cancellationTokenSource == null)
                        {
                            MessageBox.Show("Execution completed, press ok to exit.");
                        }
                        
                            //Application.Exit();

                        }
                }


                
                catch (Exception ex)
                {
                    LogMessage.Log(ex.Message);
                    LogMessage.Log(ex.StackTrace);
                }
                finally
                {
                if (GlobalVariable.dtSummaryTable.Rows.Count > 0)
                {
                    LogMessage.Log("Writing to summary file");
                    LabelText.UpdateText(Textlabel, "Generating summary file.");
                    string fileName = GlobalVariable.configDict["Summary file path"].ToString().Trim() + "_" + DateTime.Now.ToString("dd-MM-yyyy_hhmm").ToString() + ".xlsx";
                    DataExportClass.WriteExcelFile(fileName, GlobalVariable.dtSummaryTable);
                    
                }
                
                GC.Collect();
                }
                LabelText.UpdateText(Textlabel, "Process Completed.");
                MessageBox.Show("Execution completed, Click OK to  close the application");
                Application.Exit();
        }

        private void ReadInputFile(string type)
        {
            try {
                switch (type)
                {
                    case "Customer":
                        // Read Claim data
                        GlobalVariable.dtClaimData = null;
                        LogMessage.Log("Reading claim data file.");
                        LabelText.UpdateText(Textlabel, "Readng Claim data file, this may take few minutes based on the no of rows...");
                        GlobalVariable.dtClaimData = ProcessExceltoDt.ConvertToDT(GlobalVariable.configDict["ClaimDataInputFile"], "Sheet1", Textlabel);

                        //Read PST File
                        GlobalVariable.dtCustomerAgentData = null;
                        LogMessage.Log("Readng PST data file, this may take few minutes based on the no of rows...");
                        LabelText.UpdateText(Textlabel, "Readng PST data file, this may take few minutes based on the no of rows...");
                        GlobalVariable.dtCustomerAgentData = ProcessExceltoDt.ConvertToDT(GlobalVariable.configDict["Customer_AgentDataFile"], "Sheet1", Textlabel);
                        break;

                    case "Claim":
                        // Read Claim data
                        GlobalVariable.dtClaimData = null;
                        //LogMessage.Log("Reading claim data file.");
                        //LabelText.UpdateText(Textlabel, "Readng Claim data file, this may take few minutes based on the no of rows...");
                        //GlobalVariable.dtClaimData = ProcessExceltoDt.ConvertToDT(GlobalVariable.configDict["ClaimDataInputFile"], GlobalVariable.configDict["ClaimDataInputFileSheetName"], Textlabel);
                        /*try
                        {
                            GlobalVariable.dtClaimData = GlobalVariable.dtClaimData.AsEnumerable()
                               .Where(r => r.Field<string>(GlobalVariable.mappingDict["PolicyNumber"].ToString().Trim()).ToString().Trim() != "")
                               .CopyToDataTable();
                        }
                       catch(Exception de)
                        {
                           LogMessage.Log("Error: " + de);

                        }*/

                        // Read Claim2 data
                        GlobalVariable.dtCustomerAgentData = null;
                        LogMessage.Log("Reading Settlement sheet from tool input file data file.");
                        LabelText.UpdateText(Textlabel, "Readng Claim data file, this may take few minutes based on the no of rows...");
                        GlobalVariable.dtCustomerAgentData = ProcessExceltoDt.ConvertToDT(GlobalVariable.configDict["ClaimDataInputFileTwo"], GlobalVariable.configDict["ClaimDataInputFileTwoSheetName"], Textlabel);
                        //settlement sheet data is being stored in dtCustomerAgentData (tool input sheet settlement sheet)
                       //if(GlobalVariable.configDict["ClaimType"].ToString().Trim()=="Marine")
                       // {
                       //     GlobalVariable.dtVesselData = null;
                       //     LogMessage.Log("Reading Vessel risk info file.");
                       //     LabelText.UpdateText(Textlabel, "Readng Vessel risk info, this may take few minutes based on the no of rows...");
                       //     GlobalVariable.dtVesselData = ProcessExceltoDt.ConvertToDT(GlobalVariable.configDict["VesselInfoFile"], GlobalVariable.configDict["VesselInfoFileSheetName"], Textlabel);
                       // }




                        break;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error: Form1 > ReadInputFile" + ex.Message);
                LogMessage.Log("Error: Form1 > ReadInputFile" + ex.StackTrace);
            }
        }

        private void ReadNavigationAndMapping(string type, Label label)
        {
            string path = Environment.CurrentDirectory + "\\RequiredFiles\\";
            string fullPath = Path.Combine(path, type + "NavigationMotor.xlsx");
            //string fullPath = Path.Combine(path, type + "Navigation.xlsx");
            GlobalVariable.errorStatus = false;
            DataTable dataTable = new DataTable();
            LabelText.UpdateText(label, "Reading navigation file.");
            
            //Navigation File
            dataTable = null;
            dataTable = ProcessExceltoDt.ConvertToDT(fullPath, "Navigation", label);
            if (GlobalVariable.errorStatus)
            {
                LogMessage.Log("Error While Reading Navigaton.xlsx file");
                MessageBox.Show("Error while reading navigation file, Check log for more information.");
                Application.Exit();

            }
            GlobalVariable.navigationDict = ConvertDtToDict.ConvertToDictionary(dataTable);
            if (GlobalVariable.errorStatus)
            {
                LogMessage.Log("Error while creating dictionary for  Navigaton.xlsx file");
                MessageBox.Show("Error while processing navigation file, Check log for more information.");
                Application.Exit();

            }

            //Mapping Files
            //fullPath = Path.Combine(path, type + "Mapping.xlsx");
            fullPath = Path.Combine(path, type + "MappingMotor.xlsx");
            LabelText.UpdateText(label, "Reading Mapping file.");
            dataTable = null;
            dataTable = ProcessExceltoDt.ConvertToDT(fullPath, "Mapping", label);

            if (GlobalVariable.errorStatus)
            {
                LogMessage.Log("Error While Reading Mapping.xlsx file");
                MessageBox.Show("Error while reading Mapping file, Check log for more information.");
                Application.Exit();

            }
            GlobalVariable.mappingDict = ConvertDtToDict.ConvertToDictionary(dataTable);
            if (GlobalVariable.errorStatus)
            {
                LogMessage.Log("Error While creating the dictionary for mapping file");
                MessageBox.Show("Error while processing Mapping file, Check log for more information.");
                Application.Exit();

            }




            //DefaultValues
            GlobalVariable.errorStatus = false;
            dataTable = null;
            dataTable = ProcessExceltoDt.ConvertToDT(fullPath, "DefaultValues", Textlabel);
            if(dataTable.Rows.Count > 0)
                GlobalVariable.defaultValues = ConvertDtToDict.ConvertToDictionary(dataTable);
            GlobalVariable.errorStatus = false;

            //default values is not required for motor insurance



            //Country Mapping not required as of now so commenting it now for motor insurance
            //LogMessage.Log("Reading Country mapping file.");
            //LabelText.UpdateText(Textlabel, "Readng Country mapping file.");
            //path = path+"CountryCodeMapping.xlsx";
            //fullPath = Path.Combine(path, "CountryCodeMapping.xlsx");
            //dataTable = null;
            //dataTable = ProcessExceltoDt.ConvertToDT(fullPath, "Sheet1", Textlabel);
            //GlobalVariable.countryDict = ConvertDtToDict.ConvertToDictionary(dataTable);
            //if (GlobalVariable.errorStatus)
            //{
            //    LogMessage.Log("Error While creating the dictionary for CountryCodeMapping file");
            //    MessageBox.Show("Error while processing Mapping file, Check log for more information.");
            //    Application.Exit();

            //}
            //LoginNavigationValues is required to login into the portal
            GlobalVariable.errorStatus = false;
            dataTable = null;
            fullPath= Path.Combine(path, "LoginNavigationMotor.xlsx");
            dataTable = ProcessExceltoDt.ConvertToDT(fullPath, "Sheet1", Textlabel);
            GlobalVariable.LoginNavigation = ConvertDtToDict.ConvertToDictionary(dataTable);



        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            /*  //Select folder
              FolderBrowserDialog folderDlg = new FolderBrowserDialog();
              folderDlg.ShowNewFolderButton = true;
              // Show the FolderBrowserDialog.  
              DialogResult result = folderDlg.ShowDialog();
              if (result == DialogResult.OK)
              {
                  textBoxFilePath.Text = folderDlg.SelectedPath;
                  //Environment.SpecialFolder root = folderDlg.RootFolder;

              }*/
            //File dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.Title = "Select input excel file";
            openFileDialog.DefaultExt = "xlsx";
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text=openFileDialog.FileName;
            }
            MessageBox.Show(textBoxFilePath.Text);
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
           
        }

        private void EMVcheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void endorsementButton_CheckedChanged(object sender, EventArgs e)
        {
            textBoxFilePath.Enabled = true;
            buttonBrowse.Enabled = true;

        }

        private void radioButtonClaim_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
