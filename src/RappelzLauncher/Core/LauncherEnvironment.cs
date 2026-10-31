using System;
using System.Collections.Generic;
using System.Text;

namespace RappelzLauncher.Core
{
    internal static class LauncherEnvironment
    {

        public static bool TestMode { get; set; } = false;

        public static void Initialize(MainForm form)
        {
            TestMode = form._enableTestMode.Checked;
        }


    }
}
