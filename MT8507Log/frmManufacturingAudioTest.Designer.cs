﻿namespace MT8507Log
{
    partial class frmManufacturingAudioTest
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
            this.cboRmcTxSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnConnectRmcTxSerialPort = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboMtkLogSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnConnectMtkSerialPort = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtApxProjectFile = new System.Windows.Forms.TextBox();
            this.txtSpecSheetFile = new System.Windows.Forms.TextBox();
            this.btnOpenApxProjectFile = new System.Windows.Forms.Button();
            this.btnOpenSpecSheetFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cboInputMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboApxInputChannel = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cboSequenceItem = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSequenceStep = new System.Windows.Forms.TextBox();
            this.txtMeasurement = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rtxtOperating = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnOpenArduinoSourceFile = new System.Windows.Forms.Button();
            this.txtArduinoSourceFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "RMC TX Serial Port";
            // 
            // cboRmcTxSerialPorts
            // 
            this.cboRmcTxSerialPorts.FormattingEnabled = true;
            this.cboRmcTxSerialPorts.Location = new System.Drawing.Point(171, 10);
            this.cboRmcTxSerialPorts.Name = "cboRmcTxSerialPorts";
            this.cboRmcTxSerialPorts.Size = new System.Drawing.Size(96, 24);
            this.cboRmcTxSerialPorts.TabIndex = 1;
            // 
            // btnConnectRmcTxSerialPort
            // 
            this.btnConnectRmcTxSerialPort.Location = new System.Drawing.Point(267, 10);
            this.btnConnectRmcTxSerialPort.Name = "btnConnectRmcTxSerialPort";
            this.btnConnectRmcTxSerialPort.Size = new System.Drawing.Size(75, 23);
            this.btnConnectRmcTxSerialPort.TabIndex = 2;
            this.btnConnectRmcTxSerialPort.Text = "CONNECT";
            this.btnConnectRmcTxSerialPort.UseVisualStyleBackColor = true;
            this.btnConnectRmcTxSerialPort.Click += new System.EventHandler(this.btnConnectRmcTxSerialPort_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "MTK Log Serial Port";
            // 
            // cboMtkLogSerialPorts
            // 
            this.cboMtkLogSerialPorts.FormattingEnabled = true;
            this.cboMtkLogSerialPorts.Location = new System.Drawing.Point(171, 43);
            this.cboMtkLogSerialPorts.Name = "cboMtkLogSerialPorts";
            this.cboMtkLogSerialPorts.Size = new System.Drawing.Size(96, 24);
            this.cboMtkLogSerialPorts.TabIndex = 1;
            // 
            // btnConnectMtkSerialPort
            // 
            this.btnConnectMtkSerialPort.Location = new System.Drawing.Point(267, 43);
            this.btnConnectMtkSerialPort.Name = "btnConnectMtkSerialPort";
            this.btnConnectMtkSerialPort.Size = new System.Drawing.Size(75, 23);
            this.btnConnectMtkSerialPort.TabIndex = 2;
            this.btnConnectMtkSerialPort.Text = "CONNECT";
            this.btnConnectMtkSerialPort.UseVisualStyleBackColor = true;
            this.btnConnectMtkSerialPort.Click += new System.EventHandler(this.btnConnectMtkSerialPort_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(351, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "APx Project File";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(351, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Spec. Sheet File";
            // 
            // txtApxProjectFile
            // 
            this.txtApxProjectFile.Location = new System.Drawing.Point(493, 37);
            this.txtApxProjectFile.Name = "txtApxProjectFile";
            this.txtApxProjectFile.ReadOnly = true;
            this.txtApxProjectFile.Size = new System.Drawing.Size(635, 26);
            this.txtApxProjectFile.TabIndex = 3;
            this.txtApxProjectFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtSpecSheetFile
            // 
            this.txtSpecSheetFile.Location = new System.Drawing.Point(493, 66);
            this.txtSpecSheetFile.Name = "txtSpecSheetFile";
            this.txtSpecSheetFile.ReadOnly = true;
            this.txtSpecSheetFile.Size = new System.Drawing.Size(635, 26);
            this.txtSpecSheetFile.TabIndex = 3;
            this.txtSpecSheetFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnOpenApxProjectFile
            // 
            this.btnOpenApxProjectFile.Location = new System.Drawing.Point(1126, 36);
            this.btnOpenApxProjectFile.Name = "btnOpenApxProjectFile";
            this.btnOpenApxProjectFile.Size = new System.Drawing.Size(44, 28);
            this.btnOpenApxProjectFile.TabIndex = 2;
            this.btnOpenApxProjectFile.Text = "...";
            this.btnOpenApxProjectFile.UseVisualStyleBackColor = true;
            this.btnOpenApxProjectFile.Click += new System.EventHandler(this.btnOpenApxProjectFile_Click);
            // 
            // btnOpenSpecSheetFile
            // 
            this.btnOpenSpecSheetFile.Location = new System.Drawing.Point(1126, 65);
            this.btnOpenSpecSheetFile.Name = "btnOpenSpecSheetFile";
            this.btnOpenSpecSheetFile.Size = new System.Drawing.Size(44, 28);
            this.btnOpenSpecSheetFile.TabIndex = 4;
            this.btnOpenSpecSheetFile.Text = "...";
            this.btnOpenSpecSheetFile.UseVisualStyleBackColor = true;
            this.btnOpenSpecSheetFile.Click += new System.EventHandler(this.btnOpenSpecSheetFile_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(399, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Input Mode";
            // 
            // cboInputMode
            // 
            this.cboInputMode.FormattingEnabled = true;
            this.cboInputMode.Items.AddRange(new object[] {
            "AUX",
            "OPTICAL",
            "HDMI IN",
            "HDMI ARC",
            "BLUETOOTH",
            "USB"});
            this.cboInputMode.Location = new System.Drawing.Point(494, 95);
            this.cboInputMode.Name = "cboInputMode";
            this.cboInputMode.Size = new System.Drawing.Size(145, 24);
            this.cboInputMode.TabIndex = 1;
            this.cboInputMode.SelectedIndexChanged += new System.EventHandler(this.cboInputMode_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(657, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "APx Input Channels";
            // 
            // cboApxInputChannel
            // 
            this.cboApxInputChannel.FormattingEnabled = true;
            this.cboApxInputChannel.Items.AddRange(new object[] {
            "FL,FR,Sub 3ch (2.1)",
            "FL,FR,Ctr,FTL,FTR,Sub 6ch (3.1.2)",
            "FL,FR,Ctr,FTL,FTR,SL,SR,Sub 8ch (5.1.2)",
            "FL,Ctr,FTL,SL,SR,RTL,Sub 7ch (5.1.4)"});
            this.cboApxInputChannel.Location = new System.Drawing.Point(806, 95);
            this.cboApxInputChannel.Name = "cboApxInputChannel";
            this.cboApxInputChannel.Size = new System.Drawing.Size(364, 24);
            this.cboApxInputChannel.TabIndex = 1;
            this.cboApxInputChannel.SelectedIndexChanged += new System.EventHandler(this.cboApxInputChannel_SelectedIndexChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // cboSequenceItem
            // 
            this.cboSequenceItem.FormattingEnabled = true;
            this.cboSequenceItem.Location = new System.Drawing.Point(16, 144);
            this.cboSequenceItem.Name = "cboSequenceItem";
            this.cboSequenceItem.Size = new System.Drawing.Size(532, 24);
            this.cboSequenceItem.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(224, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "Signal Paths && Measurements";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "Sequence Step";
            // 
            // txtSequenceStep
            // 
            this.txtSequenceStep.Location = new System.Drawing.Point(127, 181);
            this.txtSequenceStep.Name = "txtSequenceStep";
            this.txtSequenceStep.ReadOnly = true;
            this.txtSequenceStep.Size = new System.Drawing.Size(100, 26);
            this.txtSequenceStep.TabIndex = 3;
            this.txtSequenceStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMeasurement
            // 
            this.txtMeasurement.Location = new System.Drawing.Point(233, 181);
            this.txtMeasurement.Name = "txtMeasurement";
            this.txtMeasurement.ReadOnly = true;
            this.txtMeasurement.Size = new System.Drawing.Size(315, 26);
            this.txtMeasurement.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(552, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(184, 16);
            this.label9.TabIndex = 0;
            this.label9.Text = "Operating Instructions";
            // 
            // rtxtOperating
            // 
            this.rtxtOperating.Enabled = false;
            this.rtxtOperating.Location = new System.Drawing.Point(555, 145);
            this.rtxtOperating.Name = "rtxtOperating";
            this.rtxtOperating.Size = new System.Drawing.Size(611, 144);
            this.rtxtOperating.TabIndex = 6;
            this.rtxtOperating.Text = "";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(351, 14);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(136, 16);
            this.label10.TabIndex = 0;
            this.label10.Text = "Arduino Src File";
            // 
            // btnOpenArduinoSourceFile
            // 
            this.btnOpenArduinoSourceFile.Location = new System.Drawing.Point(1126, 7);
            this.btnOpenArduinoSourceFile.Name = "btnOpenArduinoSourceFile";
            this.btnOpenArduinoSourceFile.Size = new System.Drawing.Size(44, 28);
            this.btnOpenArduinoSourceFile.TabIndex = 2;
            this.btnOpenArduinoSourceFile.Text = "...";
            this.btnOpenArduinoSourceFile.UseVisualStyleBackColor = true;
            this.btnOpenArduinoSourceFile.Click += new System.EventHandler(this.BtnOpenArduinoSourceFile_Click);
            // 
            // txtArduinoSourceFile
            // 
            this.txtArduinoSourceFile.Location = new System.Drawing.Point(493, 8);
            this.txtArduinoSourceFile.Name = "txtArduinoSourceFile";
            this.txtArduinoSourceFile.ReadOnly = true;
            this.txtArduinoSourceFile.Size = new System.Drawing.Size(635, 26);
            this.txtArduinoSourceFile.TabIndex = 3;
            this.txtArduinoSourceFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // frmManufacturingAudioTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 842);
            this.Controls.Add(this.rtxtOperating);
            this.Controls.Add(this.cboSequenceItem);
            this.Controls.Add(this.btnOpenSpecSheetFile);
            this.Controls.Add(this.txtSpecSheetFile);
            this.Controls.Add(this.txtMeasurement);
            this.Controls.Add(this.txtSequenceStep);
            this.Controls.Add(this.txtArduinoSourceFile);
            this.Controls.Add(this.txtApxProjectFile);
            this.Controls.Add(this.btnConnectMtkSerialPort);
            this.Controls.Add(this.btnOpenArduinoSourceFile);
            this.Controls.Add(this.btnOpenApxProjectFile);
            this.Controls.Add(this.btnConnectRmcTxSerialPort);
            this.Controls.Add(this.cboApxInputChannel);
            this.Controls.Add(this.cboInputMode);
            this.Controls.Add(this.cboMtkLogSerialPorts);
            this.Controls.Add(this.cboRmcTxSerialPorts);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("GulimChe", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmManufacturingAudioTest";
            this.Text = "Manufacturing AUDIO TEST";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmManufacturingAudioTest_FormClosing);
            this.Load += new System.EventHandler(this.frmManufacturingAudioTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboRmcTxSerialPorts;
        private System.Windows.Forms.Button btnConnectRmcTxSerialPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboMtkLogSerialPorts;
        private System.Windows.Forms.Button btnConnectMtkSerialPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtApxProjectFile;
        private System.Windows.Forms.TextBox txtSpecSheetFile;
        private System.Windows.Forms.Button btnOpenApxProjectFile;
        private System.Windows.Forms.Button btnOpenSpecSheetFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboInputMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboApxInputChannel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox cboSequenceItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSequenceStep;
        private System.Windows.Forms.TextBox txtMeasurement;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox rtxtOperating;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnOpenArduinoSourceFile;
        private System.Windows.Forms.TextBox txtArduinoSourceFile;
    }
}