using System;
using System.IO;
using System.Threading.Tasks;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Files
    {
        public static async Task ScanAsync()
        {
            CheckIfExists(Program.FpsUnlockerExePath);
            CheckIfExists(Program.InjectorPath);
            CheckIfExists(Program.ReShadePath);

            if (!File.Exists(Program.FpsUnlockerCfgPath)) await FpsUnlockerCfg.RunAsync();
        }

        private static void CheckIfExists(string filePath)
        {
            if (!File.Exists(filePath)) Default._status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, filePath)}\n";
        }

        public static void DeleteSetupAsync()
        {
            if (!File.Exists(NormalRelease.SetupPathExe)) return;

            try
            {
                File.Delete(NormalRelease.SetupPathExe);
                Default._status_Label.Text += $"{Resources.Default_DeletedOldSetupFromTempDirectory}\n";
                Program.Logger.Info($"Deleted old setup file from temp folder: {NormalRelease.SetupPathExe}");
            }
            catch (Exception ex)
            {
                Default._status_Label.Text += $"[x] {ex.Message}\n";
                Program.Logger.Error(ex.ToString());
            }
        }
    }
}
