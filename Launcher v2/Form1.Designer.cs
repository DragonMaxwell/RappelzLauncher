namespace Launcher_v3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.strtGameBtn = new System.Windows.Forms.Button();
            this.downloadLbl = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.Button();
            this.minimizeBtn = new System.Windows.Forms.Button();
            this.patchNotes = new System.Windows.Forms.WebBrowser();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.progressBar1.ForeColor = System.Drawing.Color.DarkRed;
            this.progressBar1.Location = new System.Drawing.Point(18, 646);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1060, 35);
            this.progressBar1.TabIndex = 0;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // strtGameBtn
            // 
            this.strtGameBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.strtGameBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(203)))));
            this.strtGameBtn.FlatAppearance.BorderSize = 2;
            this.strtGameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.strtGameBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.strtGameBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(203)))));
            this.strtGameBtn.Location = new System.Drawing.Point(1089, 597);
            this.strtGameBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.strtGameBtn.Name = "strtGameBtn";
            this.strtGameBtn.Size = new System.Drawing.Size(243, 86);
            this.strtGameBtn.TabIndex = 1;
            this.strtGameBtn.Text = "START GAME";
            this.strtGameBtn.UseVisualStyleBackColor = false;
            this.strtGameBtn.Click += new System.EventHandler(this.strtGameBtn_Click);
            // 
            // downloadLbl
            // 
            this.downloadLbl.BackColor = System.Drawing.Color.Gray;
            this.downloadLbl.ForeColor = System.Drawing.Color.Transparent;
            this.downloadLbl.Location = new System.Drawing.Point(18, 608);
            this.downloadLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.downloadLbl.Name = "downloadLbl";
            this.downloadLbl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.downloadLbl.Size = new System.Drawing.Size(1060, 33);
            this.downloadLbl.TabIndex = 2;
            this.downloadLbl.Text = "Client is up to date";
            this.downloadLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.BackgroundImage = global::Launcher_v3.Properties.Resources.close1;
            this.closeBtn.FlatAppearance.BorderSize = 0;
            this.closeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeBtn.Location = new System.Drawing.Point(1299, 0);
            this.closeBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(51, 40);
            this.closeBtn.TabIndex = 3;
            this.closeBtn.UseVisualStyleBackColor = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            this.closeBtn.MouseEnter += new System.EventHandler(this.closeBtn_MouseEnter);
            this.closeBtn.MouseLeave += new System.EventHandler(this.closeBtn_MouseLeave);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.BackgroundImage = global::Launcher_v3.Properties.Resources.minimize1;
            this.minimizeBtn.FlatAppearance.BorderSize = 0;
            this.minimizeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeBtn.Location = new System.Drawing.Point(1248, 0);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(51, 40);
            this.minimizeBtn.TabIndex = 4;
            this.minimizeBtn.UseVisualStyleBackColor = false;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
            this.minimizeBtn.MouseEnter += new System.EventHandler(this.minimizeBtn_MouseEnter);
            this.minimizeBtn.MouseLeave += new System.EventHandler(this.minimizeBtn_MouseLeave);
            // 
            // patchNotes
            // 
            this.patchNotes.Location = new System.Drawing.Point(0, 50);
            this.patchNotes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.patchNotes.MinimumSize = new System.Drawing.Size(30, 31);
            this.patchNotes.Name = "patchNotes";
            this.patchNotes.ScrollBarsEnabled = false;
            this.patchNotes.Size = new System.Drawing.Size(1350, 640);
            this.patchNotes.TabIndex = 5;
            this.patchNotes.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(17, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(210, 26);
            this.textBox1.TabIndex = 6;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(17, 140);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(210, 26);
            this.textBox2.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Account";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Password";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(1089, 333);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(243, 230);
            this.panel1.TabIndex = 10;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(17, 183);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(141, 24);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Use AutoLogin";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "AutoLogin";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(1350, 692);
            this.Controls.Add(this.downloadLbl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.strtGameBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.patchNotes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button strtGameBtn;
        private System.Windows.Forms.Label downloadLbl;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button minimizeBtn;
        private System.Windows.Forms.WebBrowser patchNotes;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

