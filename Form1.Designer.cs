
namespace III_ProjectOne
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBarInfo = new System.Windows.Forms.ProgressBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.Textlabel = new System.Windows.Forms.Label();
            this.radioButtonCustomer = new System.Windows.Forms.RadioButton();
            this.radioButtonAgent = new System.Windows.Forms.RadioButton();
            this.radioButtonClaim = new System.Windows.Forms.RadioButton();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.toolTipBrowse = new System.Windows.Forms.ToolTip(this.components);
            this.EMVcheckBox = new System.Windows.Forms.CheckBox();
            this.endorsementButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(12, 165);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(58, 15);
            this.labelProgress.TabIndex = 0;
            this.labelProgress.Text = "Progress: ";
            this.labelProgress.Click += new System.EventHandler(this.labelProgress_Click);
            // 
            // progressBarInfo
            // 
            this.progressBarInfo.Location = new System.Drawing.Point(12, 183);
            this.progressBarInfo.Name = "progressBarInfo";
            this.progressBarInfo.Size = new System.Drawing.Size(540, 13);
            this.progressBarInfo.TabIndex = 1;
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonStart.Location = new System.Drawing.Point(16, 213);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 25);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // Textlabel
            // 
            this.Textlabel.AutoSize = true;
            this.Textlabel.Location = new System.Drawing.Point(76, 165);
            this.Textlabel.Name = "Textlabel";
            this.Textlabel.Size = new System.Drawing.Size(0, 15);
            this.Textlabel.TabIndex = 3;
            // 
            // radioButtonCustomer
            // 
            this.radioButtonCustomer.AutoSize = true;
            this.radioButtonCustomer.Location = new System.Drawing.Point(16, 46);
            this.radioButtonCustomer.Name = "radioButtonCustomer";
            this.radioButtonCustomer.Size = new System.Drawing.Size(77, 19);
            this.radioButtonCustomer.TabIndex = 4;
            this.radioButtonCustomer.TabStop = true;
            this.radioButtonCustomer.Text = "Customer";
            this.radioButtonCustomer.UseVisualStyleBackColor = true;
            this.radioButtonCustomer.CheckedChanged += new System.EventHandler(this.radioButtonCustomer_CheckedChanged);
            // 
            // radioButtonAgent
            // 
            this.radioButtonAgent.AutoSize = true;
            this.radioButtonAgent.Location = new System.Drawing.Point(123, 47);
            this.radioButtonAgent.Name = "radioButtonAgent";
            this.radioButtonAgent.Size = new System.Drawing.Size(57, 19);
            this.radioButtonAgent.TabIndex = 5;
            this.radioButtonAgent.TabStop = true;
            this.radioButtonAgent.Text = "Agent";
            this.radioButtonAgent.UseVisualStyleBackColor = true;
            this.radioButtonAgent.CheckedChanged += new System.EventHandler(this.radioButtonAgent_CheckedChanged);
            // 
            // radioButtonClaim
            // 
            this.radioButtonClaim.AutoSize = true;
            this.radioButtonClaim.Location = new System.Drawing.Point(210, 47);
            this.radioButtonClaim.Name = "radioButtonClaim";
            this.radioButtonClaim.Size = new System.Drawing.Size(56, 19);
            this.radioButtonClaim.TabIndex = 6;
            this.radioButtonClaim.TabStop = true;
            this.radioButtonClaim.Text = "Claim";
            this.radioButtonClaim.UseVisualStyleBackColor = true;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(123, 213);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(76, 25);
            this.buttonStop.TabIndex = 7;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(16, 84);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(536, 23);
            this.textBoxFilePath.TabIndex = 8;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(16, 116);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 9;
            this.buttonBrowse.Text = "Browse";
            this.toolTipBrowse.SetToolTip(this.buttonBrowse, "My Tool Tip");
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // toolTipBrowse
            // 
            this.toolTipBrowse.IsBalloon = true;
            this.toolTipBrowse.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipBrowse.ToolTipTitle = "Info";
            this.toolTipBrowse.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // EMVcheckBox
            // 
            this.EMVcheckBox.AutoSize = true;
            this.EMVcheckBox.Location = new System.Drawing.Point(117, 120);
            this.EMVcheckBox.Name = "EMVcheckBox";
            this.EMVcheckBox.Size = new System.Drawing.Size(166, 19);
            this.EMVcheckBox.TabIndex = 10;
            this.EMVcheckBox.Text = "Enable Manual Verification";
            this.EMVcheckBox.UseVisualStyleBackColor = true;
            this.EMVcheckBox.CheckedChanged += new System.EventHandler(this.EMVcheckBox_CheckedChanged);
            // 
            // endorsementButton
            // 
            this.endorsementButton.AutoSize = true;
            this.endorsementButton.Location = new System.Drawing.Point(293, 49);
            this.endorsementButton.Name = "endorsementButton";
            this.endorsementButton.Size = new System.Drawing.Size(131, 19);
            this.endorsementButton.TabIndex = 11;
            this.endorsementButton.TabStop = true;
            this.endorsementButton.Text = "Check Endorsement";
            this.endorsementButton.UseVisualStyleBackColor = true;
            this.endorsementButton.CheckedChanged += new System.EventHandler(this.endorsementButton_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 356);
            this.Controls.Add(this.endorsementButton);
            this.Controls.Add(this.EMVcheckBox);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.radioButtonClaim);
            this.Controls.Add(this.radioButtonAgent);
            this.Controls.Add(this.radioButtonCustomer);
            this.Controls.Add(this.Textlabel);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressBarInfo);
            this.Controls.Add(this.labelProgress);
            this.Name = "Form1";
            this.Text = "III ProjectOne";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressBarInfo;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label Textlabel;
        private System.Windows.Forms.RadioButton radioButtonCustomer;
        private System.Windows.Forms.RadioButton radioButtonAgent;
        private System.Windows.Forms.RadioButton radioButtonClaim;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.ToolTip toolTipBrowse;
        private System.Windows.Forms.CheckBox EMVcheckBox;
        private System.Windows.Forms.RadioButton endorsementButton;
    }
}

