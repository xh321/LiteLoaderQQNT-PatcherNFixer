namespace LiteLoaderPatchNFixer
{
    partial class FrmMain
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPatchQQ = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblQQDir = new System.Windows.Forms.Label();
            this.btnFixQQCorrupt = new System.Windows.Forms.Button();
            this.lblQQVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPatchQQ
            // 
            this.btnPatchQQ.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPatchQQ.Location = new System.Drawing.Point(229, 88);
            this.btnPatchQQ.Name = "btnPatchQQ";
            this.btnPatchQQ.Size = new System.Drawing.Size(205, 62);
            this.btnPatchQQ.TabIndex = 0;
            this.btnPatchQQ.Text = "一键修补 QQ";
            this.btnPatchQQ.UseVisualStyleBackColor = true;
            this.btnPatchQQ.Click += new System.EventHandler(this.btnPatchQQ_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.Location = new System.Drawing.Point(12, 425);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(109, 20);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "状态：等待";
            // 
            // lblQQDir
            // 
            this.lblQQDir.AutoSize = true;
            this.lblQQDir.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblQQDir.Location = new System.Drawing.Point(12, 354);
            this.lblQQDir.Name = "lblQQDir";
            this.lblQQDir.Size = new System.Drawing.Size(129, 20);
            this.lblQQDir.TabIndex = 2;
            this.lblQQDir.Text = "QQ路径：未知";
            // 
            // btnFixQQCorrupt
            // 
            this.btnFixQQCorrupt.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnFixQQCorrupt.Location = new System.Drawing.Point(229, 190);
            this.btnFixQQCorrupt.Name = "btnFixQQCorrupt";
            this.btnFixQQCorrupt.Size = new System.Drawing.Size(205, 62);
            this.btnFixQQCorrupt.TabIndex = 4;
            this.btnFixQQCorrupt.Text = "一键修复 QQ 提示损坏";
            this.btnFixQQCorrupt.UseVisualStyleBackColor = true;
            this.btnFixQQCorrupt.Click += new System.EventHandler(this.btnFixQQCorrupt_Click);
            // 
            // lblQQVersion
            // 
            this.lblQQVersion.AutoSize = true;
            this.lblQQVersion.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblQQVersion.Location = new System.Drawing.Point(12, 385);
            this.lblQQVersion.Name = "lblQQVersion";
            this.lblQQVersion.Size = new System.Drawing.Size(129, 20);
            this.lblQQVersion.TabIndex = 5;
            this.lblQQVersion.Text = "QQ版本：未知";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 457);
            this.Controls.Add(this.lblQQVersion);
            this.Controls.Add(this.btnFixQQCorrupt);
            this.Controls.Add(this.lblQQDir);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnPatchQQ);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LiteLoader Patcher & Fixer";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPatchQQ;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblQQDir;
        private System.Windows.Forms.Button btnFixQQCorrupt;
        private System.Windows.Forms.Label lblQQVersion;
    }
}

