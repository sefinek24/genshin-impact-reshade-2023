using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using StellaLauncher.Forms;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class DeleteBenefits
    {
        public static void RunAsync()
        {
            // Delete presets for patrons
            string presets = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons");
            if (Directory.Exists(presets)) DeleteDirectory(presets);

            // Delete addons for patrons
            string addons = Path.Combine(Default.ResourcesPath, "ReShade", "Addons");
            if (Directory.Exists(addons)) DeleteDirectory(addons);

            // Delete 3DMigoto files
            string migotoDir = Path.Combine(Default.ResourcesPath, "3DMigoto");
            string[] filesToDelete = { "d3d11.dll", "d3dcompiler_46.dll", "loader.exe", "nvapi64.dll" };
            DeleteFiles(migotoDir, filesToDelete);

            // Delete cmd files: data/cmd
            string cmdPath = Path.Combine(Program.AppPath, "data", "cmd");
            if (Directory.Exists(cmdPath))
            {
                string[] cmdFilesToDelete = { "run.cmd", "run-patrons.cmd" };
                DeleteFiles(cmdPath, cmdFilesToDelete);

                // Delete: data/cmd/start
                string cmdDir = Path.Combine(cmdPath, "start");
                if (Directory.Exists(cmdDir)) DeleteDirectory(cmdDir);
            }

            // Delete key
            DeleteRegistryKey();
        }


        // Delete specific files in a folder
        private static void DeleteFiles(string folderPath, IEnumerable<string> filesToDelete)
        {
            Log.Output($"Deleting files in folder: {folderPath}");

            try
            {
                foreach (string fileName in filesToDelete)
                {
                    string filePath = Path.Combine(folderPath, fileName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Log.Output($"Deleted file: {fileName}");
                    }
                    else
                    {
                        Log.Output($"File not found: {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting files in folder: {Path.GetDirectoryName(folderPath)}");
                Log.SaveError(ex.ToString());
            }
        }

        // Delete a directory and its contents
        private static void DeleteDirectory(string directoryPath)
        {
            Log.Output($"Deleting files in folder: {directoryPath}");

            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting folder: {Path.GetDirectoryName(directoryPath)}");
                Log.SaveError(ex.ToString());
            }
        }

        // Delete registry key for patrons
        private static void DeleteRegistryKey()
        {
            const string secret = "Secret";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Secret.RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(secret);
                        Log.Output($"Deleted key `{secret}` from the registry.");
                    }
                    else
                    {
                        Log.Output($"Registry key `{secret}` not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting registry key `{secret}`.");
                Log.SaveError(ex.ToString());
            }
        }
    }
}
