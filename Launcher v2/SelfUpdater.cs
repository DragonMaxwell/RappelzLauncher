using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher_v3
{
    public class SelfUpdater
    {
        public static void CheckAndApplyUpdate()
        {
            try
            {
                var launcherName = $@"{Assembly.GetExecutingAssembly().GetName().Name}.exe";
                var currentLauncherPath = AppDomain.CurrentDomain.BaseDirectory;
                var newLauncherPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource");

                var currentFile = Path.Combine(currentLauncherPath, launcherName);
                var newFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource", launcherName);
                
                if (!File.Exists(newFile))
                {
                    return;
                }

                File.Copy(newFile, $@"{currentFile}.new");

                ApplyUpdate(currentFile, newFile);

                CleanOldFiles(currentLauncherPath, launcherName);
                
                /*
                string currentExe = Assembly.GetExecutingAssembly().Location;
                string currentDir = Path.GetDirectoryName(currentExe);
                string exeName = Path.GetFileName(currentExe);
                string updateFile = Path.Combine(currentDir, $"{exeName}.new");
                // Se existe uma atualização, aplica
                if (File.Exists(updateFile))
                {
                    ApplyUpdate(currentExe, updateFile);
                    return; // Sai após aplicar a atualização
                }

                // Limpa arquivos antigos
                CleanOldFiles(currentDir, exeName);
                */
                
            }
            catch
            {
                // Ignora erros silenciosamente
            }
        }

        private static void ApplyUpdate(string currentExe, string updateFile)
        {
            string currentDir = Path.GetDirectoryName(currentExe);
            string exeName = Path.GetFileName(currentExe);
            string tempOld = Path.Combine(currentDir, $"{exeName}.old");

            try
            {
                // 1. Renomeia o executável atual para .old
                if (File.Exists(currentExe))
                {
                    File.Move(currentExe, tempOld);
                }

                // 2. Renomeia o .new para o nome original
                File.Move(updateFile, currentExe);

                // 3. Inicia o novo executável
                Process.Start(new ProcessStartInfo
                {
                    FileName = currentExe,
                    WorkingDirectory = currentDir,
                    UseShellExecute = true
                });

                Application.Exit();
            }
            catch
            {
                // Se falhar, restaura do backup
                try
                {
                    if (File.Exists(tempOld) && !File.Exists(currentExe))
                    {
                        File.Move(tempOld, currentExe);
                    }
                }
                catch { }
            }
        }

        private static void CleanOldFiles(string directory, string exeName)
        {
            try
            {
                // Deleta arquivos .old antigos
                string oldFile = Path.Combine(directory, $"{exeName}.old");
                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }

                // Deleta arquivos .new que porventura existam
                string newFile = Path.Combine(directory, $"{exeName}.new");
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
            }
            catch
            {
                // Ignora erros na limpeza
            }
        }
    }
}




