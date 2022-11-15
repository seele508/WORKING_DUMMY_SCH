
namespace SchTracer
{
    partial class ParentForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParentForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnMainMenu = new System.Windows.Forms.Button();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelChildForm = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblSchName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCurrTime = new System.Windows.Forms.Label();
            this.btnMin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblLastRun = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelChildForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnSetting);
            this.panel1.Controls.Add(this.btnMainMenu);
            this.panel1.Controls.Add(this.panelLogo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(270, 632);
            this.panel1.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(144)))), ((int)(((byte)(241)))));
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(0, 586);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(270, 46);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(144)))), ((int)(((byte)(241)))));
            this.btnSetting.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSetting.FlatAppearance.BorderSize = 0;
            this.btnSetting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(144)))), ((int)(((byte)(241)))));
            this.btnSetting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.OrangeRed;
            this.btnSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetting.ForeColor = System.Drawing.Color.White;
            this.btnSetting.Image = global::SchTracer.Properties.Resources.gear1;
            this.btnSetting.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSetting.Location = new System.Drawing.Point(0, 200);
            this.btnSetting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.btnSetting.Size = new System.Drawing.Size(270, 46);
            this.btnSetting.TabIndex = 2;
            this.btnSetting.Text = "Settings";
            this.btnSetting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSetting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSetting.UseVisualStyleBackColor = false;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnMainMenu
            // 
            this.btnMainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(144)))), ((int)(((byte)(241)))));
            this.btnMainMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMainMenu.FlatAppearance.BorderSize = 0;
            this.btnMainMenu.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(144)))), ((int)(((byte)(241)))));
            this.btnMainMenu.FlatAppearance.MouseOverBackColor = System.Drawing.Color.OrangeRed;
            this.btnMainMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainMenu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMainMenu.ForeColor = System.Drawing.Color.White;
            this.btnMainMenu.Image = global::SchTracer.Properties.Resources.gear1;
            this.btnMainMenu.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMainMenu.ImageKey = "(none)";
            this.btnMainMenu.Location = new System.Drawing.Point(0, 154);
            this.btnMainMenu.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainMenu.Name = "btnMainMenu";
            this.btnMainMenu.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.btnMainMenu.Size = new System.Drawing.Size(270, 46);
            this.btnMainMenu.TabIndex = 2;
            this.btnMainMenu.Text = "Main Menu";
            this.btnMainMenu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMainMenu.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMainMenu.UseVisualStyleBackColor = false;
            this.btnMainMenu.Click += new System.EventHandler(this.btnMainMenu_Click);
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.panelLogo.Controls.Add(this.pictureBox1);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(270, 154);
            this.panelLogo.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(14, 23);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(252, 97);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelChildForm
            // 
            this.panelChildForm.BackColor = System.Drawing.Color.White;
            this.panelChildForm.Controls.Add(this.label2);
            this.panelChildForm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelChildForm.ForeColor = System.Drawing.SystemColors.Control;
            this.panelChildForm.Location = new System.Drawing.Point(270, 154);
            this.panelChildForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelChildForm.Name = "panelChildForm";
            this.panelChildForm.Size = new System.Drawing.Size(906, 478);
            this.panelChildForm.TabIndex = 2;
            this.panelChildForm.Paint += new System.Windows.Forms.PaintEventHandler(this.panelChildForm_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(326, 92);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(274, 64);
            this.label2.TabIndex = 0;
            this.label2.Text = "Welcome to Scheduler, \r\nclick Main Menu to start";
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblSchName
            // 
            this.lblSchName.AutoSize = true;
            this.lblSchName.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSchName.ForeColor = System.Drawing.Color.White;
            this.lblSchName.Location = new System.Drawing.Point(298, 18);
            this.lblSchName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSchName.Name = "lblSchName";
            this.lblSchName.Size = new System.Drawing.Size(202, 48);
            this.lblSchName.TabIndex = 3;
            this.lblSchName.Text = "SCH_NAME";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(303, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Current Time :";
            // 
            // lblCurrTime
            // 
            this.lblCurrTime.AutoSize = true;
            this.lblCurrTime.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrTime.ForeColor = System.Drawing.Color.White;
            this.lblCurrTime.Location = new System.Drawing.Point(436, 78);
            this.lblCurrTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrTime.Name = "lblCurrTime";
            this.lblCurrTime.Size = new System.Drawing.Size(102, 25);
            this.lblCurrTime.TabIndex = 5;
            this.lblCurrTime.Text = "lblCurrTime";
            this.lblCurrTime.Click += new System.EventHandler(this.lblCurrTime_Click);
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.FlatAppearance.BorderSize = 0;
            this.btnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.OrangeRed;
            this.btnMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMin.ForeColor = System.Drawing.Color.White;
            this.btnMin.Location = new System.Drawing.Point(1124, 0);
            this.btnMin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(52, 38);
            this.btnMin.TabIndex = 6;
            this.btnMin.Text = " - ";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(670, 78);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last Run :";
            // 
            // lblLastRun
            // 
            this.lblLastRun.AutoSize = true;
            this.lblLastRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastRun.ForeColor = System.Drawing.Color.White;
            this.lblLastRun.Location = new System.Drawing.Point(759, 78);
            this.lblLastRun.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastRun.Name = "lblLastRun";
            this.lblLastRun.Size = new System.Drawing.Size(93, 25);
            this.lblLastRun.TabIndex = 8;
            this.lblLastRun.Text = "lblLastRun";
            // 
            // ParentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(1176, 632);
            this.Controls.Add(this.lblLastRun);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.lblCurrTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSchName);
            this.Controls.Add(this.panelChildForm);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ParentForm";
            this.Text = "NewFlatUI";
            this.panel1.ResumeLayout(false);
            this.panelLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelChildForm.ResumeLayout(false);
            this.panelChildForm.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnMainMenu;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panelChildForm;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblSchName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCurrTime;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblLastRun;
        private System.Windows.Forms.Label label2;
    }
}