using Microsoft.Web.WebView2.WinForms;

namespace RappelzLauncher;

partial class MainForm
{
    private Panel headerPanel = null!;
    private Label headerTitle = null!;
    private Panel centerPanel = null!;
    private WebView2 _webView = null!;
    private Panel loginPanel = null!;
    private Label accountLabel = null!;
    private TextBox _accountInput = null!;
    private Label passwordLabel = null!;
    private TextBox _passwordInput = null!;
    private CheckBox _autoLoginInput = null!;
    private Panel footerPanel = null!;
    private Panel progressPanel = null!;
    private Panel startPanel = null!;
    private Label _downloadFileLabel = null!;
    private Label _downloadPercentLabel = null!;
    private Panel _progressTrack = null!;
    private Panel _progressFill = null!;
    private Button _startButton = null!;

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        headerTitle = new Label();
        centerPanel = new Panel();
        _webView = new WebView2();
        loginPanel = new Panel();
        _startButton = new Button();
        _autoLoginInput = new CheckBox();
        _passwordInput = new TextBox();
        passwordLabel = new Label();
        _accountInput = new TextBox();
        accountLabel = new Label();
        footerPanel = new Panel();
        progressPanel = new Panel();
        _downloadFileLabel = new Label();
        _downloadPercentLabel = new Label();
        _progressTrack = new Panel();
        _progressFill = new Panel();
        startPanel = new Panel();
        _enableTestMode = new CheckBox();
        headerPanel.SuspendLayout();
        centerPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_webView).BeginInit();
        loginPanel.SuspendLayout();
        footerPanel.SuspendLayout();
        progressPanel.SuspendLayout();
        _progressTrack.SuspendLayout();
        SuspendLayout();
        // 
        // headerPanel
        // 
        headerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        headerPanel.BackColor = Color.FromArgb(24, 34, 51);
        headerPanel.BorderStyle = BorderStyle.FixedSingle;
        headerPanel.Controls.Add(_enableTestMode);
        headerPanel.Controls.Add(headerTitle);
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(1485, 46);
        headerPanel.TabIndex = 0;
        // 
        // headerTitle
        // 
        headerTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
        headerTitle.ForeColor = Color.White;
        headerTitle.Location = new Point(3, 0);
        headerTitle.Name = "headerTitle";
        headerTitle.Size = new Size(326, 44);
        headerTitle.TabIndex = 0;
        headerTitle.Text = "RAPPELZ  |  LAUNCHER";
        headerTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // centerPanel
        // 
        centerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        centerPanel.BackColor = Color.FromArgb(12, 17, 26);
        centerPanel.Controls.Add(_webView);
        centerPanel.Location = new Point(0, 46);
        centerPanel.Margin = new Padding(0);
        centerPanel.Name = "centerPanel";
        centerPanel.Size = new Size(1225, 565);
        centerPanel.TabIndex = 2;
        // 
        // _webView
        // 
        _webView.AllowExternalDrop = false;
        _webView.CreationProperties = null;
        _webView.DefaultBackgroundColor = Color.FromArgb(12, 17, 26);
        _webView.Dock = DockStyle.Fill;
        _webView.Location = new Point(0, 0);
        _webView.Margin = new Padding(0);
        _webView.Name = "_webView";
        _webView.Size = new Size(1225, 565);
        _webView.TabIndex = 0;
        _webView.ZoomFactor = 1D;
        // 
        // loginPanel
        // 
        loginPanel.BackColor = Color.FromArgb(25, 35, 52);
        loginPanel.Controls.Add(_startButton);
        loginPanel.Controls.Add(_autoLoginInput);
        loginPanel.Controls.Add(_passwordInput);
        loginPanel.Controls.Add(passwordLabel);
        loginPanel.Controls.Add(_accountInput);
        loginPanel.Controls.Add(accountLabel);
        loginPanel.Location = new Point(1225, 46);
        loginPanel.Name = "loginPanel";
        loginPanel.Size = new Size(260, 667);
        loginPanel.TabIndex = 3;
        // 
        // _startButton
        // 
        _startButton.BackColor = Color.Transparent;
        _startButton.BackgroundImage = Properties.Resources.startButtonImage;
        _startButton.BackgroundImageLayout = ImageLayout.Stretch;
        _startButton.FlatAppearance.BorderSize = 0;
        _startButton.FlatStyle = FlatStyle.Flat;
        _startButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _startButton.ForeColor = Color.Transparent;
        _startButton.Location = new Point(12, 551);
        _startButton.Name = "_startButton";
        _startButton.Size = new Size(240, 106);
        _startButton.TabIndex = 0;
        _startButton.UseVisualStyleBackColor = true;
        // 
        // _autoLoginInput
        // 
        _autoLoginInput.ForeColor = Color.White;
        _autoLoginInput.Location = new Point(11, 198);
        _autoLoginInput.Name = "_autoLoginInput";
        _autoLoginInput.Size = new Size(222, 34);
        _autoLoginInput.TabIndex = 0;
        _autoLoginInput.Text = "Use AutoLogin";
        // 
        // _passwordInput
        // 
        _passwordInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _passwordInput.Font = new Font("Segoe UI", 11F);
        _passwordInput.Location = new Point(11, 138);
        _passwordInput.Name = "_passwordInput";
        _passwordInput.Size = new Size(239, 37);
        _passwordInput.TabIndex = 1;
        _passwordInput.UseSystemPasswordChar = true;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoSize = true;
        passwordLabel.ForeColor = Color.White;
        passwordLabel.Location = new Point(5, 105);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(93, 28);
        passwordLabel.TabIndex = 2;
        passwordLabel.Text = "Password";
        // 
        // _accountInput
        // 
        _accountInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _accountInput.Font = new Font("Segoe UI", 11F);
        _accountInput.Location = new Point(11, 49);
        _accountInput.Name = "_accountInput";
        _accountInput.Size = new Size(239, 37);
        _accountInput.TabIndex = 3;
        // 
        // accountLabel
        // 
        accountLabel.AutoSize = true;
        accountLabel.ForeColor = Color.White;
        accountLabel.Location = new Point(6, 15);
        accountLabel.Name = "accountLabel";
        accountLabel.Size = new Size(84, 28);
        accountLabel.TabIndex = 4;
        accountLabel.Text = "Account";
        // 
        // footerPanel
        // 
        footerPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        footerPanel.BackColor = Color.FromArgb(25, 35, 52);
        footerPanel.BorderStyle = BorderStyle.FixedSingle;
        footerPanel.Controls.Add(progressPanel);
        footerPanel.Controls.Add(startPanel);
        footerPanel.Location = new Point(0, 611);
        footerPanel.Name = "footerPanel";
        footerPanel.Size = new Size(1485, 106);
        footerPanel.TabIndex = 4;
        // 
        // progressPanel
        // 
        progressPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        progressPanel.Controls.Add(_downloadFileLabel);
        progressPanel.Controls.Add(_downloadPercentLabel);
        progressPanel.Controls.Add(_progressTrack);
        progressPanel.Location = new Point(0, 0);
        progressPanel.Name = "progressPanel";
        progressPanel.Size = new Size(1225, 104);
        progressPanel.TabIndex = 0;
        // 
        // _downloadFileLabel
        // 
        _downloadFileLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _downloadFileLabel.ForeColor = Color.White;
        _downloadFileLabel.Location = new Point(27, 10);
        _downloadFileLabel.Name = "_downloadFileLabel";
        _downloadFileLabel.Size = new Size(1051, 37);
        _downloadFileLabel.TabIndex = 0;
        _downloadFileLabel.Text = "Checking for updates...";
        _downloadFileLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _downloadPercentLabel
        // 
        _downloadPercentLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _downloadPercentLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _downloadPercentLabel.ForeColor = Color.WhiteSmoke;
        _downloadPercentLabel.Location = new Point(1127, 10);
        _downloadPercentLabel.Name = "_downloadPercentLabel";
        _downloadPercentLabel.Size = new Size(98, 37);
        _downloadPercentLabel.TabIndex = 1;
        _downloadPercentLabel.TextAlign = ContentAlignment.MiddleRight;
        // 
        // _progressTrack
        // 
        _progressTrack.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _progressTrack.BackColor = Color.FromArgb(12, 18, 28);
        _progressTrack.BorderStyle = BorderStyle.FixedSingle;
        _progressTrack.Controls.Add(_progressFill);
        _progressTrack.Location = new Point(27, 50);
        _progressTrack.Name = "_progressTrack";
        _progressTrack.Size = new Size(1198, 34);
        _progressTrack.TabIndex = 2;
        // 
        // _progressFill
        // 
        _progressFill.BackColor = Color.FromArgb(13, 129, 22);
        _progressFill.Location = new Point(0, 0);
        _progressFill.Name = "_progressFill";
        _progressFill.Size = new Size(0, 20);
        _progressFill.TabIndex = 0;
        // 
        // startPanel
        // 
        startPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        startPanel.Location = new Point(1225, 0);
        startPanel.Name = "startPanel";
        startPanel.Size = new Size(258, 104);
        startPanel.TabIndex = 1;
        // 
        // _enableTestMode
        // 
        _enableTestMode.ForeColor = Color.White;
        _enableTestMode.Location = new Point(1230, 5);
        _enableTestMode.Name = "_enableTestMode";
        _enableTestMode.Size = new Size(242, 34);
        _enableTestMode.TabIndex = 1;
        _enableTestMode.Text = "Enable TestMode";
        _enableTestMode.CheckedChanged += _enableTestMode_CheckedChanged;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(11F, 28F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(12, 17, 26);
        ClientSize = new Size(1485, 717);
        Controls.Add(headerPanel);
        Controls.Add(centerPanel);
        Controls.Add(loginPanel);
        Controls.Add(footerPanel);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1080, 640);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Rappelz Launcher";
        headerPanel.ResumeLayout(false);
        centerPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_webView).EndInit();
        loginPanel.ResumeLayout(false);
        loginPanel.PerformLayout();
        footerPanel.ResumeLayout(false);
        progressPanel.ResumeLayout(false);
        _progressTrack.ResumeLayout(false);
        ResumeLayout(false);
    }

    public CheckBox _enableTestMode;
}
