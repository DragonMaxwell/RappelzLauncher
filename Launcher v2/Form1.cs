using Ionic.Zip;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Launcher_v3
{
    public partial class Form1 : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private IniFile _iniFile;
        private string sframeParams { get; set; }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();

            patchNotes.Navigated += (sender, e) =>
            {
                Debug.WriteLine("Navigated: " + e.Url);
            };

            patchNotes.DocumentCompleted += (sender, e) =>
            {
                Debug.WriteLine("DocumentCompleted: " + e.Url);
                Debug.WriteLine("ReadyState: " + patchNotes.ReadyState);
            };

            patchNotes.ScriptErrorsSuppressed = false;

            patchNotes.Visible = true;
            //patchNotes.BringToFront();

            patchNotes.DocumentText = @"
        <html>
            <body style='background:red; color:white'>
                <h1>TESTE WEBBROWSER</h1>
            </body>
        </html>";

            //Load current values from launcher.ini during initialize.
            _iniFile = new IniFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher.ini"));

            textBox1.Text = _iniFile.Account;
            textBox2.Text = _iniFile.Password;
            sframeParams = _iniFile.SframeParam;
            checkBox1.Checked = bool.TryParse(_iniFile.UseAutoLogin, out bool result) ? result : false;

            SelfUpdater.CheckAndApplyUpdate();

            this.Shown += (sender, e) => LoadBackgroundImage();

            //Download progress
            Task.Run(() => backgroundWorker1.RunWorkerAsync());

            strtGameBtn.Enabled = false;

            this.FormClosing += MainForm_FormClosing;
        }

        private void CheckLauncherUpdate()
        {
            var currentLauncher = $@"{Assembly.GetExecutingAssembly().GetName().Name}.exe";
            var newLauncher = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", currentLauncher);

            if (!File.Exists(newLauncher))
            {
                return;
            }


        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save current values on launcher.ini during FormClosing
            _iniFile.Account = textBox1.Text;
            _iniFile.Password = textBox2.Text;
            _iniFile.UseAutoLogin = checkBox1.Checked.ToString().ToLower();
            _iniFile.SframeParam = sframeParams;
            
            _iniFile.SaveAllSettings();
            _iniFile.Dispose();
        }

        //Makes the form dragable
        private void Form1_MouseDown(object sender,
        System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        //Close Button
        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeBtn_MouseEnter(object sender, EventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.close2;
        }

        private void closeBtn_MouseLeave(object sender, EventArgs e)
        {
            closeBtn.BackgroundImage = Properties.Resources.close1;
        }

        //Minimize Button
        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizeBtn_MouseEnter(object sender, EventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimize2;
        }

        private void minimizeBtn_MouseLeave(object sender, EventArgs e)
        {
            minimizeBtn.BackgroundImage = Properties.Resources.minimize1;
        }

        //Delete File
        static bool deleteFile(string f)
        {
            try
            {
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        //Load BackgroundImage from server "C:\inetpub\wwwroot\html\index.html" folder
        private void LoadBackgroundImage()
        {
            if(string.IsNullOrEmpty(_iniFile.LauncherBackgroundUrl))
            {
                return;
            }

            var url = Flurl.Url.Combine(_iniFile.LauncherBackgroundUrl, "html", "index.html");

            //patchNotes.Url = new Uri(Flurl.Url.Combine(_iniFile.LauncherBackgroundUrl, "html", "index.html"));

            //http://maxrappelzhome.duckdns.org:8080/
            //patchNotes.Refresh();

            patchNotes.DocumentText =
                "<html><body><h2>TESTE WEBBROWSER</h2></body></html>";

            patchNotes.DocumentCompleted += (sender, e) =>
            {
                if (e.Url.ToString() == "about:blank" &&
                    patchNotes.Document?.Body?.InnerText?.Contains("TESTE WEBBROWSER") == true)
                {
                    Debug.WriteLine("HTML local carregado, navegando...");
                    patchNotes.Navigate(url);
                }
            };
            

            //patchNotes.Navigate(url);

            //Debug.WriteLine("Navigate executado");
        }
        

        //background Worker: Handles downloading the updates
        private async void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //LoadBackgroundImage();

            //Defines the server's update directory
            //string Server = "http://127.0.0.1/Updates/";
            string Server = _iniFile.LauncherBackgroundUrl;

            //Defines application root
            string Root = AppDomain.CurrentDomain.BaseDirectory;

            //checks client version
            string lclVersion;
            using (StreamReader reader = new StreamReader("version"))
            {
                lclVersion = reader.ReadLine();
            }

            if (lclVersion == null)
            {
                //Make sure version file exists
                FileStream fs = null;
                if (!File.Exists("version"))
                {
                    using (fs = File.Create("version"))
                    {

                    }

                    using (StreamWriter sw = new StreamWriter("version"))
                    {
                        sw.Write("1.0");
                        lclVersion = "1.0";
                    }
                }
            }

            decimal localVersion = decimal.Parse(lclVersion);

            //server's list of updates
            //Updates.xml must be placed at root of "C:\inetpub\wwwroot" if you are using IIS.
            var httpClient = new HttpClient();
            var xml = Flurl.Url.Combine(_iniFile.LauncherBackgroundUrl, "updates.xml");

            string xmlContent = string.Empty;
            XDocument serverXml = new XDocument();

            try
            {
                //Reading Url as Xml before Parse
                xmlContent = await httpClient.GetStringAsync(xml);
                //Prefering XDocument.Parse() instead XDocument.Load() to avoid most of exceptions
                serverXml = XDocument.Parse(xmlContent, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                return;
            }
            
            //The Update Process
            foreach (XElement update in serverXml.Descendants("update"))
            {
                string version = update.Element("version").Value;
                string file = update.Element("file").Value;

                decimal serverVersion = decimal.Parse(version);


                string sUrlToReadFileFrom = Flurl.Url.Combine(Server, "resources", file);

                string sFilePathToWriteFileTo = Path.Combine(Root, file);

                if (!(serverVersion > localVersion))
                {
                    return;
                }
                
                Uri url = new Uri(sUrlToReadFileFrom);
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                response.Close();

                Int64 iSize = response.ContentLength;

                Int64 iRunningByteTotal = 0;

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    using (System.IO.Stream streamRemote = client.OpenRead(new Uri(sUrlToReadFileFrom)))
                    {
                        using (Stream streamLocal = new FileStream(sFilePathToWriteFileTo, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            int iByteSize = 0;
                            byte[] byteBuffer = new byte[iSize];
                            while ((iByteSize = streamRemote.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                            {
                                streamLocal.Write(byteBuffer, 0, iByteSize);
                                iRunningByteTotal += iByteSize;

                                double dIndex = (double)(iRunningByteTotal);
                                double dTotal = (double)byteBuffer.Length;
                                double dProgressPercentage = (dIndex / dTotal);
                                int iProgressPercentage = (int)(dProgressPercentage * 100);

                                worker.ReportProgress(iProgressPercentage);
                                patchNotes.Refresh();
                            }

                            streamLocal.Close();
                        }

                        streamRemote.Close();
                    }
                }

                backgroundWorker1 = worker;

                //unzip
                using (ZipFile zip = ZipFile.Read(file))
                {
                    foreach (ZipEntry zipFiles in zip)
                    {
                        zipFiles.Extract(Root + "\\Resource\\", true);
                    }
                }

                //Delete Zip File
                deleteFile(file);

                var currentLauncher = $@"{Assembly.GetExecutingAssembly().GetName().Name}.exe";
                var newLauncher = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", currentLauncher);

                if (File.Exists(newLauncher))
                {
                    //download new version file
                    //WebClient webClient = new WebClient();
                    //webClient.DownloadFile(Flurl.Url.Combine(Server, "resources", "version.txt"), Path.Combine(@Root, "version"));

                    MessageBox.Show(@"New Launcher has been detected, reopen the current one.", @"Information", MessageBoxButtons.OK);
                    
                    Application.Exit(e);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            downloadLbl.ForeColor = System.Drawing.Color.Silver;
            downloadLbl.Text = "Downloading Updates";
            patchNotes.Refresh();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            strtGameBtn.Enabled = true;
            //this.downloadLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(121)))), ((int)(((byte)(203)))));
            downloadLbl.Text = "Client is up to date";
            patchNotes.Refresh();
        }


        //Starts the game
        private void strtGameBtn_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                sframeParams += $" /imbclogin /account:{textBox1.Text} /password:{textBox2.Text}";
            }

            Process.Start("RappelzCmdLauncher.exe", $"{sframeParams}");
            this.Close();
        }

    }
}
