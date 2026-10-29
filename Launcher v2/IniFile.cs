using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Launcher_v3
{
    public class IniFile : IDisposable
    {
        private string filePath;
        private bool disposed = false;        

        public string Account { get; set; }
        public string Password { get; set; }
        public string UseAutoLogin { get; set; } = "true";
        public string SframeParam { get; set; }
        public string LauncherBackgroundUrl { get; set; }


        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);

        public IniFile(string path)
        {
            filePath = path;
            EnsureFileExists();
            LoadAllSettings();
        }

        private void LoadAllSettings()
        {
            Account = ReadValueFromFile("Login", "account");
            Password = ReadValueFromFile("Login", "password");
            UseAutoLogin = ReadValueFromFile("Login", "useAutoLogin");
            SframeParam = ReadValueFromFile("SframeParams", "param");
            LauncherBackgroundUrl = ReadValueFromFile("SframeParams", "LauncherBackgroundUrl");
        }

        public void SaveAllSettings()
        {
            WriteValueToFile("Login", "account", Account);
            WriteValueToFile("Login", "password", Password);
            WriteValueToFile("Login", "useAutoLogin", UseAutoLogin);            
        }

        private void EnsureFileExists()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    string directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    WriteValueToFile("Login", "account", "");
                    WriteValueToFile("Login", "password", "");
                    WriteValueToFile("Login", "useAutoLogin", UseAutoLogin);
                    WriteValueToFile("SframeParams", "param", "");
                    WriteValueToFile("SframeParams", "LauncherBackgroundUrl", "url for your 'index.html'");
                }
            }
            catch
            {
                //silent fail
            }
        }

        private void WriteValueToFile(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        private string ReadValueFromFile(string section, string key)
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, filePath);
            return retVal.ToString();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    SaveAllSettings();
                }
                disposed = true;
            }
        }

        ~IniFile()
        {
            Dispose(false);
        }
    }
}
