﻿namespace CardOperating
{
    partial class IccCardInfo
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupIccCardInfo = new System.Windows.Forms.GroupBox();
            this.AppValidDateTo = new System.Windows.Forms.DateTimePicker();
            this.AppDateFlag = new System.Windows.Forms.Label();
            this.AppValidDateFrom = new System.Windows.Forms.DateTimePicker();
            this.AppValid = new System.Windows.Forms.Label();
            this.textCompanyTo = new System.Windows.Forms.TextBox();
            this.textCompanyFrom = new System.Windows.Forms.TextBox();
            this.CompanyTo = new System.Windows.Forms.Label();
            this.CompanyFrom = new System.Windows.Forms.Label();
            this.textTermialID = new System.Windows.Forms.TextBox();
            this.TermialID = new System.Windows.Forms.Label();
            this.textPSAMNo = new System.Windows.Forms.TextBox();
            this.IccCardNo = new System.Windows.Forms.Label();
            this.SaveClose = new System.Windows.Forms.Button();
            this.groupIccCardInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupIccCardInfo
            // 
            this.groupIccCardInfo.Controls.Add(this.AppValidDateTo);
            this.groupIccCardInfo.Controls.Add(this.AppDateFlag);
            this.groupIccCardInfo.Controls.Add(this.AppValidDateFrom);
            this.groupIccCardInfo.Controls.Add(this.AppValid);
            this.groupIccCardInfo.Controls.Add(this.textCompanyTo);
            this.groupIccCardInfo.Controls.Add(this.textCompanyFrom);
            this.groupIccCardInfo.Controls.Add(this.CompanyTo);
            this.groupIccCardInfo.Controls.Add(this.CompanyFrom);
            this.groupIccCardInfo.Controls.Add(this.textTermialID);
            this.groupIccCardInfo.Controls.Add(this.TermialID);
            this.groupIccCardInfo.Controls.Add(this.textPSAMNo);
            this.groupIccCardInfo.Controls.Add(this.IccCardNo);
            this.groupIccCardInfo.Controls.Add(this.SaveClose);
            this.groupIccCardInfo.Location = new System.Drawing.Point(2, 1);
            this.groupIccCardInfo.Name = "groupIccCardInfo";
            this.groupIccCardInfo.Size = new System.Drawing.Size(395, 495);
            this.groupIccCardInfo.TabIndex = 0;
            this.groupIccCardInfo.TabStop = false;
            this.groupIccCardInfo.Text = "接触式卡信息";
            // 
            // AppValidDateTo
            // 
            this.AppValidDateTo.Location = new System.Drawing.Point(229, 161);
            this.AppValidDateTo.Name = "AppValidDateTo";
            this.AppValidDateTo.Size = new System.Drawing.Size(123, 21);
            this.AppValidDateTo.TabIndex = 13;
            // 
            // AppDateFlag
            // 
            this.AppDateFlag.AutoSize = true;
            this.AppDateFlag.Location = new System.Drawing.Point(212, 167);
            this.AppDateFlag.Name = "AppDateFlag";
            this.AppDateFlag.Size = new System.Drawing.Size(11, 12);
            this.AppDateFlag.TabIndex = 12;
            this.AppDateFlag.Text = "-";
            // 
            // AppValidDateFrom
            // 
            this.AppValidDateFrom.Location = new System.Drawing.Point(88, 162);
            this.AppValidDateFrom.Name = "AppValidDateFrom";
            this.AppValidDateFrom.Size = new System.Drawing.Size(122, 21);
            this.AppValidDateFrom.TabIndex = 11;
            // 
            // AppValid
            // 
            this.AppValid.AutoSize = true;
            this.AppValid.Location = new System.Drawing.Point(17, 166);
            this.AppValid.Name = "AppValid";
            this.AppValid.Size = new System.Drawing.Size(65, 12);
            this.AppValid.TabIndex = 10;
            this.AppValid.Text = "应用有效期";
            // 
            // textCompanyTo
            // 
            this.textCompanyTo.Location = new System.Drawing.Point(119, 129);
            this.textCompanyTo.MaxLength = 16;
            this.textCompanyTo.Name = "textCompanyTo";
            this.textCompanyTo.ReadOnly = true;
            this.textCompanyTo.Size = new System.Drawing.Size(157, 21);
            this.textCompanyTo.TabIndex = 9;
            this.textCompanyTo.Text = "35FFFFFFFFFFFFFF";
            // 
            // textCompanyFrom
            // 
            this.textCompanyFrom.Location = new System.Drawing.Point(119, 96);
            this.textCompanyFrom.MaxLength = 16;
            this.textCompanyFrom.Name = "textCompanyFrom";
            this.textCompanyFrom.ReadOnly = true;
            this.textCompanyFrom.Size = new System.Drawing.Size(157, 21);
            this.textCompanyFrom.TabIndex = 8;
            this.textCompanyFrom.Text = "10FFFFFFFFFFFFFF";
            // 
            // CompanyTo
            // 
            this.CompanyTo.AutoSize = true;
            this.CompanyTo.Location = new System.Drawing.Point(17, 133);
            this.CompanyTo.Name = "CompanyTo";
            this.CompanyTo.Size = new System.Drawing.Size(89, 12);
            this.CompanyTo.TabIndex = 7;
            this.CompanyTo.Text = "应用接收者标识";
            // 
            // CompanyFrom
            // 
            this.CompanyFrom.AutoSize = true;
            this.CompanyFrom.Location = new System.Drawing.Point(17, 100);
            this.CompanyFrom.Name = "CompanyFrom";
            this.CompanyFrom.Size = new System.Drawing.Size(89, 12);
            this.CompanyFrom.TabIndex = 6;
            this.CompanyFrom.Text = "应用发行者标识";
            // 
            // textTermialID
            // 
            this.textTermialID.Location = new System.Drawing.Point(119, 63);
            this.textTermialID.MaxLength = 12;
            this.textTermialID.Name = "textTermialID";
            this.textTermialID.Size = new System.Drawing.Size(126, 21);
            this.textTermialID.TabIndex = 4;
            this.textTermialID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textTermialID_KeyPress);
            // 
            // TermialID
            // 
            this.TermialID.AutoSize = true;
            this.TermialID.Location = new System.Drawing.Point(17, 67);
            this.TermialID.Name = "TermialID";
            this.TermialID.Size = new System.Drawing.Size(65, 12);
            this.TermialID.TabIndex = 3;
            this.TermialID.Text = "终端机编号";
            // 
            // textPSAMNo
            // 
            this.textPSAMNo.Location = new System.Drawing.Point(119, 30);
            this.textPSAMNo.MaxLength = 16;
            this.textPSAMNo.Name = "textPSAMNo";
            this.textPSAMNo.Size = new System.Drawing.Size(195, 21);
            this.textPSAMNo.TabIndex = 2;
            this.textPSAMNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textPSAMNo_KeyPress);
            // 
            // IccCardNo
            // 
            this.IccCardNo.AutoSize = true;
            this.IccCardNo.Location = new System.Drawing.Point(17, 34);
            this.IccCardNo.Name = "IccCardNo";
            this.IccCardNo.Size = new System.Drawing.Size(65, 12);
            this.IccCardNo.TabIndex = 1;
            this.IccCardNo.Text = "PSAM序列号";
            // 
            // SaveClose
            // 
            this.SaveClose.Location = new System.Drawing.Point(302, 454);
            this.SaveClose.Name = "SaveClose";
            this.SaveClose.Size = new System.Drawing.Size(75, 23);
            this.SaveClose.TabIndex = 0;
            this.SaveClose.Text = "保存";
            this.SaveClose.UseVisualStyleBackColor = true;
            this.SaveClose.Click += new System.EventHandler(this.SaveClose_Click);
            // 
            // IccCardInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 500);
            this.Controls.Add(this.groupIccCardInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IccCardInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "接触式卡信息";
            this.groupIccCardInfo.ResumeLayout(false);
            this.groupIccCardInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupIccCardInfo;
        private System.Windows.Forms.Button SaveClose;
        private System.Windows.Forms.TextBox textPSAMNo;
        private System.Windows.Forms.Label IccCardNo;
        private System.Windows.Forms.Label TermialID;
        private System.Windows.Forms.TextBox textTermialID;
        private System.Windows.Forms.Label CompanyTo;
        private System.Windows.Forms.Label CompanyFrom;
        private System.Windows.Forms.TextBox textCompanyTo;
        private System.Windows.Forms.TextBox textCompanyFrom;
        private System.Windows.Forms.Label AppValid;
        private System.Windows.Forms.DateTimePicker AppValidDateFrom;
        private System.Windows.Forms.Label AppDateFlag;
        private System.Windows.Forms.DateTimePicker AppValidDateTo;
    }
}