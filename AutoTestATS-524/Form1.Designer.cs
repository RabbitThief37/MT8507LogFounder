namespace AutoTestATS_524
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboMtkComport = new System.Windows.Forms.ComboBox();
            this.cboRmcComport = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtArduinoFileName = new System.Windows.Forms.TextBox();
            this.btnSelectArduinoFile = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtULIVersion = new System.Windows.Forms.TextBox();
            this.btnTestStart = new System.Windows.Forms.Button();
            this.txtCurrrentJob = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSuccessCounter = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFailedCounter = new System.Windows.Forms.TextBox();
            this.txtFailedLastTime = new System.Windows.Forms.TextBox();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "MTK Com Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 19);
            this.label2.TabIndex = 0;
            this.label2.Text = "RMC Com Port";
            // 
            // cboMtkComport
            // 
            this.cboMtkComport.FormattingEnabled = true;
            this.cboMtkComport.Location = new System.Drawing.Point(137, 34);
            this.cboMtkComport.Name = "cboMtkComport";
            this.cboMtkComport.Size = new System.Drawing.Size(136, 27);
            this.cboMtkComport.TabIndex = 1;
            // 
            // cboRmcComport
            // 
            this.cboRmcComport.FormattingEnabled = true;
            this.cboRmcComport.Location = new System.Drawing.Point(137, 72);
            this.cboRmcComport.Name = "cboRmcComport";
            this.cboRmcComport.Size = new System.Drawing.Size(136, 27);
            this.cboRmcComport.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "RMC Source File";
            // 
            // txtArduinoFileName
            // 
            this.txtArduinoFileName.Location = new System.Drawing.Point(17, 144);
            this.txtArduinoFileName.Name = "txtArduinoFileName";
            this.txtArduinoFileName.ReadOnly = true;
            this.txtArduinoFileName.Size = new System.Drawing.Size(607, 26);
            this.txtArduinoFileName.TabIndex = 2;
            // 
            // btnSelectArduinoFile
            // 
            this.btnSelectArduinoFile.Location = new System.Drawing.Point(620, 141);
            this.btnSelectArduinoFile.Name = "btnSelectArduinoFile";
            this.btnSelectArduinoFile.Size = new System.Drawing.Size(46, 32);
            this.btnSelectArduinoFile.TabIndex = 3;
            this.btnSelectArduinoFile.Text = "...";
            this.btnSelectArduinoFile.UseVisualStyleBackColor = true;
            this.btnSelectArduinoFile.Click += new System.EventHandler(this.BtnSelectArduinoFile_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(379, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 19);
            this.label4.TabIndex = 0;
            this.label4.Text = "Model";
            // 
            // txtModelName
            // 
            this.txtModelName.Location = new System.Drawing.Point(493, 31);
            this.txtModelName.Name = "txtModelName";
            this.txtModelName.ReadOnly = true;
            this.txtModelName.Size = new System.Drawing.Size(173, 26);
            this.txtModelName.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(379, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 19);
            this.label5.TabIndex = 0;
            this.label5.Text = "ULI Version";
            // 
            // txtULIVersion
            // 
            this.txtULIVersion.Location = new System.Drawing.Point(493, 69);
            this.txtULIVersion.Name = "txtULIVersion";
            this.txtULIVersion.ReadOnly = true;
            this.txtULIVersion.Size = new System.Drawing.Size(173, 26);
            this.txtULIVersion.TabIndex = 4;
            // 
            // btnTestStart
            // 
            this.btnTestStart.Location = new System.Drawing.Point(200, 212);
            this.btnTestStart.Name = "btnTestStart";
            this.btnTestStart.Size = new System.Drawing.Size(252, 51);
            this.btnTestStart.TabIndex = 5;
            this.btnTestStart.Text = "START";
            this.btnTestStart.UseVisualStyleBackColor = true;
            this.btnTestStart.Click += new System.EventHandler(this.BtnTestStart_Click);
            // 
            // txtCurrrentJob
            // 
            this.txtCurrrentJob.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.txtCurrrentJob.Location = new System.Drawing.Point(17, 292);
            this.txtCurrrentJob.Name = "txtCurrrentJob";
            this.txtCurrrentJob.ReadOnly = true;
            this.txtCurrrentJob.Size = new System.Drawing.Size(649, 26);
            this.txtCurrrentJob.TabIndex = 2;
            this.txtCurrrentJob.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 355);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 19);
            this.label6.TabIndex = 0;
            this.label6.Text = "Success Counter";
            // 
            // txtSuccessCounter
            // 
            this.txtSuccessCounter.Location = new System.Drawing.Point(163, 352);
            this.txtSuccessCounter.Name = "txtSuccessCounter";
            this.txtSuccessCounter.ReadOnly = true;
            this.txtSuccessCounter.Size = new System.Drawing.Size(110, 26);
            this.txtSuccessCounter.TabIndex = 4;
            this.txtSuccessCounter.Text = "0";
            this.txtSuccessCounter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(406, 355);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 19);
            this.label7.TabIndex = 0;
            this.label7.Text = "Failed Counter";
            // 
            // txtFailedCounter
            // 
            this.txtFailedCounter.ForeColor = System.Drawing.Color.Red;
            this.txtFailedCounter.Location = new System.Drawing.Point(556, 352);
            this.txtFailedCounter.Name = "txtFailedCounter";
            this.txtFailedCounter.ReadOnly = true;
            this.txtFailedCounter.Size = new System.Drawing.Size(110, 26);
            this.txtFailedCounter.TabIndex = 4;
            this.txtFailedCounter.Text = "0";
            this.txtFailedCounter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtFailedCounter.TextChanged += new System.EventHandler(this.TxtFailedCounter_TextChanged);
            // 
            // txtFailedLastTime
            // 
            this.txtFailedLastTime.ForeColor = System.Drawing.Color.DarkRed;
            this.txtFailedLastTime.Location = new System.Drawing.Point(410, 384);
            this.txtFailedLastTime.Name = "txtFailedLastTime";
            this.txtFailedLastTime.ReadOnly = true;
            this.txtFailedLastTime.Size = new System.Drawing.Size(256, 26);
            this.txtFailedLastTime.TabIndex = 4;
            // 
            // txtStartTime
            // 
            this.txtStartTime.Location = new System.Drawing.Point(17, 384);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.ReadOnly = true;
            this.txtStartTime.Size = new System.Drawing.Size(256, 26);
            this.txtStartTime.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 431);
            this.Controls.Add(this.btnTestStart);
            this.Controls.Add(this.txtStartTime);
            this.Controls.Add(this.txtFailedLastTime);
            this.Controls.Add(this.txtULIVersion);
            this.Controls.Add(this.txtFailedCounter);
            this.Controls.Add(this.txtSuccessCounter);
            this.Controls.Add(this.txtModelName);
            this.Controls.Add(this.btnSelectArduinoFile);
            this.Controls.Add(this.txtCurrrentJob);
            this.Controls.Add(this.txtArduinoFileName);
            this.Controls.Add(this.cboRmcComport);
            this.Controls.Add(this.cboMtkComport);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("NanumGothicCoding", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboMtkComport;
        private System.Windows.Forms.ComboBox cboRmcComport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtArduinoFileName;
        private System.Windows.Forms.Button btnSelectArduinoFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtULIVersion;
        private System.Windows.Forms.Button btnTestStart;
        private System.Windows.Forms.TextBox txtCurrrentJob;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSuccessCounter;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFailedCounter;
        private System.Windows.Forms.TextBox txtFailedLastTime;
        private System.Windows.Forms.TextBox txtStartTime;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

